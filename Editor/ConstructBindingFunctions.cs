using System;
using UnityEditor;
using System.Reflection;
using System.Text;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using AnarchyConstructFramework.Core.Common;

namespace AnarchyConstructFramework.Editor
{
    public class ConstructBindingFunctions
    {
        [MenuItem("Anarchy/Update Bindings")]
        static void UpdateBindings()
        {
            // Retrieve the settings for the AnarchyConstructFramework
            var settings = AnarchyConstructFrameworkEditorFunctions.GetSettings();
            string anarchyPath = settings.PathToAnarchyConstructFramework;
            string sharedFolderPath = Path.Combine(anarchyPath, "Shared");

            // Ensure the shared folder exists
            if (!Directory.Exists(sharedFolderPath))
            {
                Directory.CreateDirectory(sharedFolderPath);
            }

            // StringBuilder to collect all event strings
            StringBuilder classBuilder = new StringBuilder();

            // Start the class definition
            classBuilder.AppendLine("using UnityEngine.Events;");
            classBuilder.AppendLine();
            classBuilder.AppendLine("namespace Anarchy.Shared");
            classBuilder.AppendLine("{");
            classBuilder.AppendLine("    //This code is auto-generated. Please do not try to edit this file.");
            classBuilder.AppendLine("    public static class ConstructBindings");
            classBuilder.AppendLine("    {");

            // Find all assets of type AnarchyData in the project
            string[] guids = AssetDatabase.FindAssets("t:AnarchyData");
            foreach (var guid in guids)
            {
                // Load the AnarchyData asset at the path retrieved by the GUID
                AnarchyData data = AssetDatabase.LoadAssetAtPath<AnarchyData>(AssetDatabase.GUIDToAssetPath(guid));

                if (data != null)
                {
                    // Initialize listeners for detecting field changes
                    data.InitializeFieldListeners();

                    // Get the type of the current AnarchyData instance
                    var dataType = data.GetType();

                    // Use reflection to get all public instance fields, including inherited fields
                    FieldInfo[] fields = dataType.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
                    
                    // Loop through each field and retrieve its name and type
                    foreach (var field in fields)
                    {
                        // Generate event string for each field
                        string fieldEventString = $"        public static UnityEvent<{field.FieldType}> OnSend_{field.Name} = new UnityEvent<{field.FieldType}>();";
                        classBuilder.AppendLine(fieldEventString);
                    }
                }
            }

            // Close the class and namespace definitions
            classBuilder.AppendLine("    }");
            classBuilder.AppendLine("}");

            // Path to the output script
            string outputPath = Path.Combine(sharedFolderPath, "ConstructBindings.cs");

            // Write the generated class to the output file
            File.WriteAllText(outputPath, classBuilder.ToString());

            // Refresh the AssetDatabase to recognize the new script
            AssetDatabase.Refresh();

            // Schedule event binding after assembly reload
            EditorApplication.delayCall += SetUpConstructBindingListeners;

            Debug.Log($"ConstructBindings.cs generated at: {outputPath}");
        }

        // Method to link UnityEvents in AnarchyData to events in ConstructBindings after reload
        static void SetUpConstructBindingListeners()
        {
            try
            {
                // Find all assets of type AnarchyData in the project again
                string[] guids = AssetDatabase.FindAssets("t:AnarchyData");
                foreach (var guid in guids)
                {
                    // Load the AnarchyData asset at the path retrieved by the GUID
                    AnarchyData data = AssetDatabase.LoadAssetAtPath<AnarchyData>(AssetDatabase.GUIDToAssetPath(guid));

                    if (data != null)
                    {
                        LinkEventsToConstructBindings(data);
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Error setting up ConstructBinding listeners: " + ex.Message);
            }
        }

        static void LinkEventsToConstructBindings(AnarchyData data)
        {
            // Ensure ConstructBindings exists by checking for one of its events
            Type bindingsType = System.Type.GetType("Anarchy.Shared.ConstructBindings");

            if (bindingsType != null)
            {
                EventInfo[] events = bindingsType.GetEvents(BindingFlags.Public | BindingFlags.Static);
                foreach (var eventInfo in events)
                {
                    string fieldName = eventInfo.Name.Replace("OnSend_", "");

                    // Get the UnityEvent for the field in AnarchyData
                    var fieldEvent = data.GetFieldEvent(fieldName);
                    if (fieldEvent != null)
                    {
                        // Create a UnityAction to invoke the static event
                        UnityAction<object> unityAction = value =>
                        {
                            eventInfo.AddEventHandler(null, Delegate.CreateDelegate(eventInfo.EventHandlerType, fieldEvent, "Invoke"));
                        };

                        fieldEvent.AddListener(unityAction);
                    }
                }
            }
            else
            {
                Debug.LogError("ConstructBindings could not be found. Ensure that Update Bindings was called and ConstructBindings.cs is compiled.");
            }
        }
    }
}
