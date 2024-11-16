using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using UnityEditorInternal;

namespace Anarchy.Editor
{
    public class ConstructWizard : EditorWindow
    {
        private string _constructName;
        private AssemblyDefinitionAsset _anarchyAssemblyDefinition;
        private AssemblyDefinitionAsset _anarchyAssemblySharedDefinition;
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
        private bool _includeScenes = false;
        private bool _includeTests = false;

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

            // Fetch the Anarchy Assembly Definition if it's not already set
            if (_anarchyAssemblyDefinition == null)
            {
                _anarchyAssemblyDefinition = FindAnarchyAssemblyDefinition();
            }
            
            if (_anarchyAssemblySharedDefinition == null)
            {
                _anarchyAssemblySharedDefinition = FindAnarchySharedAssemblyDefinition();
            }

            GUILayout.Label("Select Folders to Include:");
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

        private AssemblyDefinitionAsset FindAnarchyAssemblyDefinition()
        {
            // First, try to find in the Assets folder.
            string[] guids = AssetDatabase.FindAssets("Anarchy");
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                AssemblyDefinitionAsset asmdef = AssetDatabase.LoadAssetAtPath<AssemblyDefinitionAsset>(path);
                if (asmdef != null && asmdef.name == "Anarchy")
                {
                    Debug.Log("Found Anarchy Assembly Definition in Assets at: " + path);
                    return asmdef;
                }
            }

            // If not found in Assets, check the Packages folder.
            string packageAsmdefPath = "Packages/com.risky.anarchy-construct-framework/Core/Anarchy.asmdef";
            AssemblyDefinitionAsset packageAsmdef = AssetDatabase.LoadAssetAtPath<AssemblyDefinitionAsset>(packageAsmdefPath);
            if (packageAsmdef != null)
            {
                Debug.Log("Found Anarchy Assembly Definition in Packages at: " + packageAsmdefPath);
                return packageAsmdef;
            }

            Debug.LogError("Anarchy Assembly Definition not found in Assets or Packages. Please ensure it exists in the project.");
            return null;
        }
        
        private AssemblyDefinitionAsset FindAnarchySharedAssemblyDefinition()
        {
            // First, try to find in the Assets folder.
            string[] guids = AssetDatabase.FindAssets("Anarchy.Shared");
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                AssemblyDefinitionAsset asmdef = AssetDatabase.LoadAssetAtPath<AssemblyDefinitionAsset>(path);
                if (asmdef != null && asmdef.name == "Anarchy.Shared")
                {
                    Debug.Log("Found Anarchy Shared Assembly Definition in Assets at: " + path);
                    return asmdef;
                }
            }

            // If not found in Assets, check the Packages folder.
            string packageAsmdefPath = "Packages/com.risky.anarchy-construct-framework/Shared/Anarchy.asmdef";
            AssemblyDefinitionAsset packageAsmdef = AssetDatabase.LoadAssetAtPath<AssemblyDefinitionAsset>(packageAsmdefPath);
            if (packageAsmdef != null)
            {
                Debug.Log("Found Anarchy Shared Assembly Definition in Packages at: " + packageAsmdefPath);
                return packageAsmdef;
            }

            Debug.LogError("Anarchy Shared Assembly Definition not found in Assets or Packages. Please ensure it exists in the project.");
            return null;
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
                CreateConstructDataScript();
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
        
        private void CreateConstructDataScript()
        {
            var settings = AnarchyConstructFrameworkEditorFunctions.GetSettings();
            string codeFolderPath = Path.Combine(settings.PathToConstructs, _constructName, "Code");

            string scriptPath = Path.Combine(codeFolderPath, $"{_constructName}Data.cs");
            if (!File.Exists(scriptPath))
            {
                // Write the new Data script with proper indentation
                using (StreamWriter writer = new StreamWriter(scriptPath))
                {
                    writer.WriteLine("using Anarchy.Core.Common;");
                    writer.WriteLine("using UnityEngine;");
                    writer.WriteLine();
                    writer.WriteLine($"namespace {settings.RootNamespace}.Constructs.{_constructName}");
                    writer.WriteLine("{");
                    writer.WriteLine($"\t[CreateAssetMenu(fileName = \"{_constructName}Data\", menuName = \"Anarchy/Constructs/Create {_constructName}Data\")]");
                    writer.WriteLine($"\tpublic class {_constructName}Data : AnarchyData");
                    writer.WriteLine("\t{");
                    writer.WriteLine("\t\t// Add public fields for this construct's data");
                    writer.WriteLine("\t\t// Go to Anarchy/Update Bindings to use events");
                    writer.WriteLine("\t}");
                    writer.WriteLine("}");
                }

                // Refresh AssetDatabase to recognize the new script
                AssetDatabase.Refresh();
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
                    writer.WriteLine("  \"name\": \"" + rootNamespace + ".Constructs." + _constructName + "\",");
                    writer.WriteLine("  \"rootNamespace\": \"" + rootNamespace + ".Constructs." + _constructName + "\",");
                    writer.WriteLine("  \"references\": [");

                    if (_anarchyAssemblyDefinition != null)
                    {
                        string anarchyAssemblyName = Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(_anarchyAssemblyDefinition));
                        writer.Write("    \"" + anarchyAssemblyName + "\"");
                        writer.WriteLine(",");
                    }
                    
                    if (_anarchyAssemblySharedDefinition != null)
                    {
                        string anarchyAssemblyName = Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(_anarchyAssemblySharedDefinition));
                        writer.Write("    \"" + anarchyAssemblyName + "\"");
                        if (_assemblies.Count > 0) writer.WriteLine(",");
                        else writer.WriteLine();
                    }

                    for (int i = 0; i < _assemblies.Count; i++)
                    {
                        if (_assemblies[i] != null)
                        {
                            string assemblyName = Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(_assemblies[i]));
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
