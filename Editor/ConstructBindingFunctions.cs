using System;
using UnityEditor;
using System.Reflection;
using System.Text;
using System.IO;
using UnityEngine;
using Anarchy.Core.Common;
using UnityEngine.Events;

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

            AppendAnarchyDataBindings(classBuilder);
            AppendAnarchyEventListeners(classBuilder);

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
            classBuilder.AppendLine("using Anarchy.Core.Common;");
            classBuilder.AppendLine();
            classBuilder.AppendLine("namespace Anarchy.Shared");
            classBuilder.AppendLine("{");
            classBuilder.AppendLine("    // This code is auto-generated. Please do not try to edit this file.");
            classBuilder.AppendLine("    public static class ConstructBindings");
            classBuilder.AppendLine("    {");
            return classBuilder;
        }

        static void AppendAnarchyDataBindings(StringBuilder classBuilder)
        {
            string[] guids = AssetDatabase.FindAssets("t:AnarchyData");

            foreach (var guid in guids)
            {
                AnarchyData data = AssetDatabase.LoadAssetAtPath<AnarchyData>(AssetDatabase.GUIDToAssetPath(guid));

                if (data != null)
                {
                    AppendDataFieldBindings(classBuilder, data);
                }
            }
        }

        static void AppendDataFieldBindings(StringBuilder classBuilder, AnarchyData data)
        {
            var fields = data.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);

            foreach (var field in fields)
            {
                classBuilder.AppendLine(GenerateUnityEventDeclaration(data, field));
            }
        }

        static string GenerateUnityEventDeclaration(AnarchyData data, FieldInfo field)
        {
            return $"        public static UnityEvent<{field.FieldType}> Send_{data.name}_{field.Name} = new UnityEvent<{field.FieldType}>();";
        }

        static void AppendAnarchyEventListeners(StringBuilder classBuilder)
        {
            var allAssemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in allAssemblies)
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (IsStaticAnarchyEventClass(type))
                    {
                        AppendEventListenersForType(classBuilder, type);
                    }
                }
            }
        }

        static bool IsStaticAnarchyEventClass(Type type)
        {
            return type.IsClass && type.IsAbstract && type.IsSealed && type.IsSubclassOf(typeof(AnarchyEvents));
        }

        static void AppendEventListenersForType(StringBuilder classBuilder, Type type)
        {
            var staticFields = type.GetFields(BindingFlags.Public | BindingFlags.Static);

            foreach (var field in staticFields)
            {
                if (field.FieldType.IsSubclassOf(typeof(UnityEventBase)))
                {
                    classBuilder.AppendLine(GenerateEventListenerCode(type, field));
                }
            }
        }

        static string GenerateEventListenerCode(Type type, FieldInfo field)
        {
            return $"        {type.Name}.{field.Name}.AddListener(() => Debug.Log(\"{field.Name} invoked from {type.Name}\"));";
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
