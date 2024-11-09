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
            _pathToAnarchyFolder = EditorGUILayout.TextField("Anarchy Folder Path", _pathToAnarchyFolder);
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
            }
            else
            {
                Debug.LogError("Failed to create AnarchySettings.");
            }
        }
    }
}
