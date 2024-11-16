using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using Anarchy.Core.Common;
using Anarchy.Enums;

namespace Anarchy.Editor
{
    public class ConstructBindingGenerator
    {
        [MenuItem("Anarchy/Update Bindings")]
        static void UpdateBindings()
        {
            // Path to save the generated bindings file
            string outputPath = "Assets/Anarchy/Shared/ConstructBindings.cs";

            StringBuilder classBuilder = InitializeClassBuilder();

            // Find all ScriptableObjects that extend AnarchyData
            string[] guids = AssetDatabase.FindAssets("t:ScriptableObject");
            foreach (var guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                AnarchyData data = AssetDatabase.LoadAssetAtPath<AnarchyData>(assetPath);

                if (data != null)
                {
                    AppendConstructHeader(classBuilder, data.name);
                    AppendEventBindings(classBuilder, data);
                    AppendFieldBindings(classBuilder, data);
                    classBuilder.AppendLine();  // Add a blank line between constructs
                }
            }

            CloseClassBuilder(classBuilder);

            // Write the generated bindings to the output file
            File.WriteAllText(outputPath, classBuilder.ToString());
            AssetDatabase.Refresh();
            Debug.Log($"ConstructBindings.cs generated at: {outputPath}");
        }

        static StringBuilder InitializeClassBuilder()
        {
            var classBuilder = new StringBuilder();
            classBuilder.AppendLine("using UnityEngine.Events;");
            classBuilder.AppendLine("using UnityEngine;");
            classBuilder.AppendLine();
            classBuilder.AppendLine("namespace Anarchy.Shared");
            classBuilder.AppendLine("{");
            classBuilder.AppendLine("    // This code is auto-generated. Please do not try to edit this file.");
            classBuilder.AppendLine("    public static class ConstructBindings");
            classBuilder.AppendLine("    {");
            return classBuilder;
        }

        static void AppendConstructHeader(StringBuilder classBuilder, string constructName)
        {
            classBuilder.AppendLine($"        // Events for {constructName}");
        }

        static void AppendEventBindings(StringBuilder classBuilder, AnarchyData data)
        {
            foreach (var eventData in data.anarchyEvents)
            {
                string eventType = GetUnityEventType(eventData);
                if (!string.IsNullOrEmpty(eventType))
                {
                    string eventName = $"Send_{data.name}_{eventData.eventName}";
                    classBuilder.AppendLine($"        public static {eventType} {eventName} = new {eventType}();");
                }
            }
        }

        static void AppendFieldBindings(StringBuilder classBuilder, AnarchyData data)
        {
            var type = data.GetType();
            var publicFields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);

            foreach (var field in publicFields)
            {
                string fieldType = ConvertToUnityType(field.FieldType);
                if (!string.IsNullOrEmpty(fieldType))
                {
                    string fieldEventName = $"Send_{data.name}_{field.Name}_Changed";
                    classBuilder.AppendLine($"        public static UnityEvent<{fieldType}> {fieldEventName} = new UnityEvent<{fieldType}>();");
                }
            }
        }

        static string GetUnityEventType(AnarchyEventData eventData)
        {
            List<string> parameterTypes = new List<string>();

            // Check each type and convert to C# type names
            if (eventData.type1 != AnarchyEventDataTypes.None) parameterTypes.Add(ConvertToCSharpType(eventData.type1));
            if (eventData.type2 != AnarchyEventDataTypes.None) parameterTypes.Add(ConvertToCSharpType(eventData.type2));
            if (eventData.type3 != AnarchyEventDataTypes.None) parameterTypes.Add(ConvertToCSharpType(eventData.type3));
            if (eventData.type4 != AnarchyEventDataTypes.None) parameterTypes.Add(ConvertToCSharpType(eventData.type4));

            if (parameterTypes.Count == 0) return "UnityEvent";
            return $"UnityEvent<{string.Join(", ", parameterTypes)}>";
        }

        static string ConvertToUnityType(Type type)
        {
            if (type == typeof(int)) return "int";
            if (type == typeof(float)) return "float";
            if (type == typeof(bool)) return "bool";
            if (type == typeof(string)) return "string";
            if (type == typeof(Vector3)) return "Vector3";
            if (type == typeof(Quaternion)) return "Quaternion";
            if (type == typeof(Transform)) return "Transform";
            if (type == typeof(GameObject)) return "GameObject";
            return null; // Unsupported type
        }

        static string ConvertToCSharpType(AnarchyEventDataTypes type)
        {
            switch (type)
            {
                case AnarchyEventDataTypes.Integer: return "int";
                case AnarchyEventDataTypes.String: return "string";
                case AnarchyEventDataTypes.Boolean: return "bool";
                case AnarchyEventDataTypes.Float: return "float";
                case AnarchyEventDataTypes.Vector3: return "Vector3";
                case AnarchyEventDataTypes.Quaternion: return "Quaternion";
                case AnarchyEventDataTypes.Transform: return "Transform";
                case AnarchyEventDataTypes.GameObject: return "GameObject";
                default: return null;
            }
        }

        static void CloseClassBuilder(StringBuilder classBuilder)
        {
            classBuilder.AppendLine("    }");
            classBuilder.AppendLine("}");
        }
    }
}
