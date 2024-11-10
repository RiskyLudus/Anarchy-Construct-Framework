using AnarchyConstructFramework.Core.Common;
using UnityEditor;
using System.Reflection;
using System.Text;
using System.IO;
using UnityEngine;

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

            Debug.Log($"ConstructBindings.cs generated at: {outputPath}");
        }
    }
}
