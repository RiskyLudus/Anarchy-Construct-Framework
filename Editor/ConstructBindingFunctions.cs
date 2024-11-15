using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using Anarchy.Attributes;
using Anarchy.Core.Common;

namespace Anarchy.Editor
{
    public class ConstructBindingFunctions
    {
        [MenuItem("Anarchy/Update Bindings")]
        static void UpdateBindings()
        {
            var settings = AnarchyConstructFrameworkEditorFunctions.GetSettings();
            string anarchyPath = settings.PathToAnarchyConstructFramework;
            string sharedFolderPath = Path.Combine(anarchyPath, "Shared");

            EnsureDirectoryExists(sharedFolderPath);

            StringBuilder classBuilder = InitializeClassBuilder();

            AppendStaticUnityEvents(classBuilder);
            AppendScriptableObjectFields(classBuilder);

            CloseClassBuilder(classBuilder);

            string outputPath = WriteToFile(sharedFolderPath, classBuilder.ToString());
            AssetDatabase.Refresh();

            Debug.Log($"ConstructBindings.cs generated at: {outputPath}");
        }

        static void EnsureDirectoryExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
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

        static void AppendStaticUnityEvents(StringBuilder classBuilder)
        {
            var allAssemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in allAssemblies)
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (IsStaticClass(type))
                    {
                        AppendUnityEventsInClass(classBuilder, type);
                    }
                }
            }
        }

        static bool IsStaticClass(Type type)
        {
            return type.IsClass && type.IsAbstract && type.IsSealed; // Static class check
        }

        static void AppendUnityEventsInClass(StringBuilder classBuilder, Type type)
        {
            var unityEventFields = type.GetFields(BindingFlags.Public | BindingFlags.Static)
                                       .Where(f => f.FieldType.IsSubclassOf(typeof(UnityEventBase)));

            if (unityEventFields.Any())
            {
                classBuilder.AppendLine($"        // UnityEvents in {type.FullName}");

                foreach (var field in unityEventFields)
                {
                    classBuilder.AppendLine(GenerateUnityEventBindingCode(type, field));
                }
            }
        }

        static string GenerateUnityEventBindingCode(Type type, FieldInfo field)
        {
            return $"{type.FullName}.{field.Name}.AddListener(value => Debug.Log(\"{field.Name} invoked from {type.FullName}: \" + value));";
        }

        static void AppendScriptableObjectFields(StringBuilder classBuilder)
        {
            string[] guids = AssetDatabase.FindAssets("t:ScriptableObject");

            foreach (var guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                var asset = AssetDatabase.LoadAssetAtPath<ScriptableObject>(assetPath);

                if (asset != null && asset.GetType().IsSubclassOf(typeof(AnarchyData)))
                {
                    AppendFieldsFromScriptableObject(classBuilder, asset);
                }
            }
        }

        static void AppendFieldsFromScriptableObject(StringBuilder classBuilder, ScriptableObject asset)
        {
            var type = asset.GetType();
            var annotatedFields = type.GetFields(BindingFlags.Public | BindingFlags.Instance)
                                      .Where(f => f.GetCustomAttribute<AnarchyAttribute>() != null);

            foreach (var field in annotatedFields)
            {
                classBuilder.AppendLine(GenerateScriptableObjectEventCode(asset, field));
            }
        }

        static string GenerateScriptableObjectEventCode(ScriptableObject asset, FieldInfo field)
        {
            string fieldType = ConvertToUnityType(field.FieldType);
            string constructName = asset.name;

            return $@"
        public static UnityEvent<{fieldType}> Send_{constructName}_{field.Name} = new UnityEvent<{fieldType}>();
        // Example usage: Send_{constructName}_{field.Name}.Invoke(newValue);
";
        }

        static string ConvertToUnityType(Type type)
        {
            if (type == typeof(float) || type == typeof(Single)) return "float";
            if (type == typeof(int) || type == typeof(Int32)) return "int";
            if (type == typeof(bool)) return "bool";
            if (type == typeof(string)) return "string";
            if (type == typeof(Vector3)) return "Vector3";
            if (type == typeof(Vector2)) return "Vector2";
            if (type == typeof(Quaternion)) return "Quaternion";
            return type.Name; // Fallback for other types
        }

        static void CloseClassBuilder(StringBuilder classBuilder)
        {
            classBuilder.AppendLine("    }");
            classBuilder.AppendLine("}");
        }

        static string WriteToFile(string sharedFolderPath, string classContent)
        {
            string outputPath = Path.Combine(sharedFolderPath, "ConstructBindings.cs");
            File.WriteAllText(outputPath, classContent);
            return outputPath;
        }
    }
}
