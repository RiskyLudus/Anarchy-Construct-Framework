using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using UnityEditorInternal;

namespace AnarchyConstructFramework.Editor
{
    public class ConstructWizard : EditorWindow
    {
        private string _constructName;
        private List<AssemblyDefinitionAsset> _assemblies = new List<AssemblyDefinitionAsset>();

        // Folder toggles
        private bool _includeCode = true;
        private bool _includeModels = true;
        private bool _includeMaterials = true;
        private bool _includeShaders = true;
        private bool _includePrefabs = true;
        private bool _includeTextures = true;
        private bool _includeAnimations = true;
        private bool _includeSettings = true;
        private bool _includeScenes = true;
        private bool _includeTests = true;

        [MenuItem("Anarchy/Create Construct")]
        static void Init()
        {
            ConstructWizard window = (ConstructWizard)EditorWindow.GetWindow(typeof(ConstructWizard));
            window.Show();
        }

        void OnGUI()
        {
            GUILayout.Label("Create Construct", EditorStyles.boldLabel);
            GUILayout.Space(10);
            GUILayout.Label("Enter Construct Name:");
            _constructName = EditorGUILayout.TextField(_constructName);
            GUILayout.Space(10);

            GUILayout.Label("Select Folders to Include:");
            _includeCode = EditorGUILayout.Toggle("Code", _includeCode);
            _includeModels = EditorGUILayout.Toggle("Models", _includeModels);
            _includeMaterials = EditorGUILayout.Toggle("Materials", _includeMaterials);
            _includeShaders = EditorGUILayout.Toggle("Shaders", _includeShaders);
            _includePrefabs = EditorGUILayout.Toggle("Prefabs", _includePrefabs);
            _includeTextures = EditorGUILayout.Toggle("Textures", _includeTextures);
            _includeAnimations = EditorGUILayout.Toggle("Animations", _includeAnimations);
            _includeSettings = EditorGUILayout.Toggle("Settings", _includeSettings);
            _includeScenes = EditorGUILayout.Toggle("Scenes", _includeScenes);
            _includeTests = EditorGUILayout.Toggle("Tests", _includeTests);

            GUILayout.Space(10);
            GUILayout.Label("Assembly References:");
            
            for (int i = 0; i < _assemblies.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                _assemblies[i] = (AssemblyDefinitionAsset)EditorGUILayout.ObjectField(_assemblies[i], typeof(AssemblyDefinitionAsset), false);
                if (GUILayout.Button("Remove", GUILayout.Width(60))) _assemblies.RemoveAt(i);
                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Add Assembly Reference")) _assemblies.Add(null);

            GUILayout.Space(10);

            if (GUILayout.Button("Create"))
            {
                if (!string.IsNullOrEmpty(_constructName))
                {
                    CreateConstructFolderStructure();
                    Close();
                }
                else
                {
                    Debug.LogError("Please enter a valid construct name.");
                }
            }
        }

        private void CreateConstructFolderStructure()
        {
            var settings = AnarchyConstructFrameworkEditorFunctions.GetSettings();
            string constructFolderPath = Path.Combine(settings.PathToConstructs, _constructName);

            if (!AssetDatabase.IsValidFolder(constructFolderPath))
            {
                try { AssetDatabase.CreateFolder(settings.PathToConstructs, _constructName); }
                catch (System.Exception ex) { Debug.LogError("Error creating folder: " + ex.Message); }
            }

            AssetDatabase.Refresh();

            if (_includeCode) CreateSubfolder(constructFolderPath, "Code");
            if (_includeModels) CreateSubfolder(constructFolderPath, "Models");
            if (_includeMaterials) CreateSubfolder(constructFolderPath, "Materials");
            if (_includeShaders) CreateSubfolder(constructFolderPath, "Shaders");
            if (_includePrefabs) CreateSubfolder(constructFolderPath, "Prefabs");
            if (_includeTextures) CreateSubfolder(constructFolderPath, "Textures");
            if (_includeAnimations) CreateSubfolder(constructFolderPath, "Animations");
            if (_includeSettings) CreateSubfolder(constructFolderPath, "Settings");
            if (_includeScenes) CreateSubfolder(constructFolderPath, "Scenes");
            if (_includeTests) CreateSubfolder(constructFolderPath, "Tests");

            if (_includeCode)
            {
                string codeFolderPath = Path.Combine(constructFolderPath, "Code");
                CreateAssemblyDefinitionFile(codeFolderPath, settings.RootNamespace);
            }

            AssetDatabase.Refresh();
        }

        private void CreateSubfolder(string parentFolder, string folderName)
        {
            string subfolderPath = Path.Combine(parentFolder, folderName);
            if (!AssetDatabase.IsValidFolder(subfolderPath))
            {
                try { AssetDatabase.CreateFolder(parentFolder, folderName); }
                catch (System.Exception ex) { Debug.LogError("Error creating folder: " + ex.Message); }
            }
        }

        private void CreateAssemblyDefinitionFile(string folderPath, string rootNamespace)
        {
            string asmdefFileName = $"{_constructName}.asmdef";
            string asmdefFilePath = Path.Combine(folderPath, asmdefFileName);

            if (!File.Exists(asmdefFilePath))
            {
                using (StreamWriter writer = new StreamWriter(asmdefFilePath))
                {
                    writer.WriteLine("{");
                    writer.WriteLine("  \"name\": \"" + _constructName + "\",");
                    writer.WriteLine("  \"rootNamespace\": \"" + rootNamespace + ".Constructs." + _constructName + "\",");

                    // Properly handle assembly references by writing only their names
                    writer.WriteLine("  \"references\": [");
                    for (int i = 0; i < _assemblies.Count; i++)
                    {
                        if (_assemblies[i] != null)
                        {
                            // Get the assembly name directly from the asset
                            string asmdefPath = AssetDatabase.GetAssetPath(_assemblies[i]);
                            string assemblyName = Path.GetFileNameWithoutExtension(asmdefPath); // Extract the name

                            writer.Write("    \"" + assemblyName + "\"");
                            if (i < _assemblies.Count - 1) writer.WriteLine(",");
                            else writer.WriteLine();
                        }
                    }
                    writer.WriteLine("  ],");

                    writer.WriteLine("  \"includePlatforms\": [],");
                    writer.WriteLine("  \"excludePlatforms\": [],");
                    writer.WriteLine("  \"allowUnsafeCode\": false,");
                    writer.WriteLine("  \"overrideReferences\": false,");
                    writer.WriteLine("  \"precompiledReferences\": [],");
                    writer.WriteLine("  \"autoReferenced\": true,");
                    writer.WriteLine("  \"defineConstraints\": [],");
                    writer.WriteLine("  \"versionDefines\": []");
                    writer.WriteLine("}");
                }
            }
        }
    }
}
