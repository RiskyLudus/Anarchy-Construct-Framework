using System.IO;
using AnarchyConstructFramework.Core.ScriptableObjects;
using UnityEngine;
using UnityEditor;

namespace AnarchyConstructFramework.Editor
{
    public class AnarchySetupWizard : EditorWindow
    {
        private string _constructFolderLocation = "Assets/Anarchy-Construct-Framework/_Constructs";
        private string _pathToAnarchyFolder = "Assets/Anarchy-Construct-Framework";
        private string _rootNamespace = "AnarchyConstructFramework";
        
        [MenuItem("Anarchy/Setup Wizard")]
        static void Init()
        {
            AnarchySetupWizard window = (AnarchySetupWizard)EditorWindow.GetWindow(typeof(AnarchySetupWizard));
            window.Show();
        }

        void OnGUI()
        {
            GUILayout.Label("Welcome to Anarchy!", EditorStyles.boldLabel);
            GUILayout.Space(10);
            GUILayout.Label("Where should we place our folder?");
            _pathToAnarchyFolder = EditorGUILayout.TextField("Anarchy Folder Path (we already assume it's under Assets)", _pathToAnarchyFolder);
            GUILayout.Label("Where should the construct folder be placed?");
            _constructFolderLocation = EditorGUILayout.TextField("Construct Folder Path", _constructFolderLocation);
            GUILayout.Label("What namespace would you like to generate with?");
            _rootNamespace = EditorGUILayout.TextField("Namespace", _rootNamespace);
            GUILayout.Space(10);
            if (GUILayout.Button("Create Anarchy Folder"))
            {
                CreateAnarchyFolderStructure();
            }
        }

        private void CreateAnarchyFolderStructure()
        {
            // Ensure the base Anarchy folder exists in the Assets directory
            if (!AssetDatabase.IsValidFolder(_pathToAnarchyFolder))
            {
                string[] folders = _pathToAnarchyFolder.Split('/');
                string parentFolder = "Assets";
                
                // Recursively create folders to match the path
                for (int i = 1; i < folders.Length; i++)
                {
                    string folderPath = Path.Combine(parentFolder, folders[i]);
                    if (!AssetDatabase.IsValidFolder(folderPath))
                    {
                        AssetDatabase.CreateFolder(parentFolder, folders[i]);
                    }
                    parentFolder = folderPath;
                }
            }

            AssetDatabase.Refresh();
            CreateConstructFolder();
        }

        private void CreateConstructFolder()
        {
            // Ensure the Construct folder exists in the specified path
            if (!AssetDatabase.IsValidFolder(_constructFolderLocation))
            {
                string[] folders = _constructFolderLocation.Split('/');
                string parentFolder = "Assets";

                for (int i = 1; i < folders.Length; i++)
                {
                    string folderPath = Path.Combine(parentFolder, folders[i]);
                    if (!AssetDatabase.IsValidFolder(folderPath))
                    {
                        AssetDatabase.CreateFolder(parentFolder, folders[i]);
                    }
                    parentFolder = folderPath;
                }
            }

            AssetDatabase.Refresh();
            CreateSharedFolderAndAsmdef();
            CreateAnarchySettings();
        }

        private void CreateAnarchySettings()
        {
            // Ensure the AnarchySettings object is created if it doesn't exist
            var settings = ScriptableObject.CreateInstance<AnarchySettings>();
            if (settings != null)
            {
                AssetDatabase.CreateAsset(settings, Path.Combine(_pathToAnarchyFolder, "AnarchySettings.asset"));
                settings.PathToAnarchyConstructFramework = _pathToAnarchyFolder;
                settings.PathToConstructs = _constructFolderLocation;
                settings.RootNamespace = _rootNamespace;
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            else
            {
                Debug.LogError("Failed to create AnarchySettings.");
            }
        }

        private void CreateSharedFolderAndAsmdef()
        {
            // Define the Shared folder path
            string sharedFolderPath = Path.Combine(_pathToAnarchyFolder, "Shared");

            // Create the Shared folder if it doesn't exist
            if (!AssetDatabase.IsValidFolder(sharedFolderPath))
            {
                AssetDatabase.CreateFolder(_pathToAnarchyFolder, "Shared");
            }

            // Locate the AnarchyConstructFramework asmdef
            string[] asmdefGuids = AssetDatabase.FindAssets("AnarchyConstructFramework t:asmdef");
            string anarchyAsmdefReference = null;

            if (asmdefGuids.Length > 0)
            {
                // Get the path to the first matched asmdef file
                anarchyAsmdefReference = Path.GetFileNameWithoutExtension(AssetDatabase.GUIDToAssetPath(asmdefGuids[0]));
            }
            else
            {
                Debug.LogError("AnarchyConstructFramework asmdef file not found. Ensure it exists in the project.");
                return;
            }

            // Define the asmdef content with the reference to AnarchyConstructFramework
            string asmdefContent = $@"
{{
    ""name"": ""Anarchy.Shared"",
    ""rootNamespace"": ""Anarchy.Shared"",
    ""references"": [
        ""{anarchyAsmdefReference}""
    ],
    ""includePlatforms"": [],
    ""excludePlatforms"": [],
    ""allowUnsafeCode"": false,
    ""overrideReferences"": false,
    ""precompiledReferences"": [],
    ""autoReferenced"": true,
    ""defineConstraints"": [],
    ""versionDefines"": [],
    ""noEngineReferences"": false
}}";

            // Write the asmdef file to the Shared folder
            string asmdefPath = Path.Combine(sharedFolderPath, "Anarchy.Shared.asmdef");
            File.WriteAllText(asmdefPath, asmdefContent);

            // Refresh the AssetDatabase to recognize the new asmdef file
            AssetDatabase.Refresh();
            Debug.Log("Shared folder and asmdef created with reference to AnarchyConstructFramework.");
        }
    }
}
