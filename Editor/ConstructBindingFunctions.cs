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
            string[] guids = AssetDatabase.FindAssets("t:AnarchyData");
            foreach (var guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                AnarchyData data = AssetDatabase.LoadAssetAtPath<AnarchyData>(assetPath);

                if (data != null)
                {
                    AppendConstructHeader(classBuilder, data.name);
                    AppendEventBindings(classBuilder, data);
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
            if (data.anarchyEvents != null && data.anarchyEvents.Length > 0)
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
            if (type == typeof(Vector2)) return "Vector2";
            if (type == typeof(Quaternion)) return "Quaternion";
            if (type == typeof(Color)) return "Color";
            if (type == typeof(Transform)) return "Transform";
            if (type == typeof(GameObject)) return "GameObject";
            if (type == typeof(Sprite)) return "Sprite";
            if (type == typeof(AudioClip)) return "AudioClip";
            if (type == typeof(Material)) return "Material";
            if (type == typeof(Texture2D)) return "Texture2D";
            if (type == typeof(RectTransform)) return "RectTransform";
            if (type == typeof(Collider)) return "Collider";
            if (type == typeof(Rigidbody)) return "Rigidbody";
            if (type == typeof(byte)) return "byte";
            if (type == typeof(byte[])) return "byte[]";
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
                case AnarchyEventDataTypes.Vector2: return "Vector2";
                case AnarchyEventDataTypes.Quaternion: return "Quaternion";
                case AnarchyEventDataTypes.Color: return "Color";
                case AnarchyEventDataTypes.Transform: return "Transform";
                case AnarchyEventDataTypes.GameObject: return "GameObject";
                case AnarchyEventDataTypes.Sprite: return "Sprite";
                case AnarchyEventDataTypes.AudioClip: return "AudioClip";
                case AnarchyEventDataTypes.Material: return "Material";
                case AnarchyEventDataTypes.Texture2D: return "Texture2D";
                case AnarchyEventDataTypes.RectTransform: return "RectTransform";
                case AnarchyEventDataTypes.Collider: return "Collider";
                case AnarchyEventDataTypes.Rigidbody: return "Rigidbody";
                case AnarchyEventDataTypes.Byte: return "byte";
                case AnarchyEventDataTypes.ByteArray: return "byte[]";
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
