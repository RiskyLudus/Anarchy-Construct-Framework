using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace AnarchyConstructFramework.Core.Common
{
    public class ConstructLoader : MonoBehaviour
    {
        private static readonly Dictionary<string, AsyncOperationHandle<GameObject>> loadedPrefabs = new Dictionary<string, AsyncOperationHandle<GameObject>>();
        private static readonly Dictionary<string, AsyncOperationHandle<SceneInstance>> loadedScenes = new Dictionary<string, AsyncOperationHandle<SceneInstance>>();

        // Load and instantiate a prefab asynchronously
        public static void LoadPrefabConstructAsync(string prefabAddress, System.Action<GameObject> onPrefabLoaded = null)
        {
            if (!loadedPrefabs.ContainsKey(prefabAddress))
            {
                AsyncOperationHandle<GameObject> handle = Addressables.InstantiateAsync(prefabAddress);
                handle.Completed += operationHandle =>
                {
                    if (operationHandle.Status == AsyncOperationStatus.Succeeded)
                    {
                        loadedPrefabs[prefabAddress] = handle; // Store handle
                        onPrefabLoaded?.Invoke(handle.Result);
                        Debug.Log($"Prefab '{prefabAddress}' loaded and instantiated successfully.");
                    }
                    else
                    {
                        Debug.LogError($"Failed to load and instantiate prefab '{prefabAddress}'. Error: {operationHandle.OperationException}");
                    }
                };
            }
            else
            {
                Debug.LogWarning($"Prefab '{prefabAddress}' is already loaded.");
            }
        }

        // Unload a prefab

        public static void UnloadPrefabConstruct(string prefabAddress)
        {
            if (loadedPrefabs.TryGetValue(prefabAddress, out AsyncOperationHandle<GameObject> handle))
            {
                Addressables.ReleaseInstance(handle.Result);
                loadedPrefabs.Remove(prefabAddress);
                Debug.Log($"Prefab '{prefabAddress}' unloaded successfully.");
            }
            else
            {
                Debug.LogWarning($"Prefab '{prefabAddress}' is not loaded, so it cannot be unloaded.");
            }
        }


        // Load a scene additively asynchronously
        public static void LoadSceneConstructAsync(string sceneAddress, System.Action<string> onSceneLoaded = null)
        {
            if (!loadedScenes.ContainsKey(sceneAddress))
            {
                AsyncOperationHandle<SceneInstance> handle = Addressables.LoadSceneAsync(sceneAddress, LoadSceneMode.Additive);
                handle.Completed += operationHandle =>
                {
                    if (operationHandle.Status == AsyncOperationStatus.Succeeded)
                    {
                        loadedScenes[sceneAddress] = handle; // Store handle
                        onSceneLoaded?.Invoke(sceneAddress);
                        Debug.Log($"Scene '{sceneAddress}' loaded successfully.");
                    }
                    else
                    {
                        Debug.LogError($"Failed to load scene '{sceneAddress}'. Error: {operationHandle.OperationException}");
                    }
                };
            }
            else
            {
                Debug.LogWarning($"Scene '{sceneAddress}' is already loaded.");
            }
        }

        // Unload a scene
        public static void UnloadSceneConstruct(string sceneAddress)
        {
            if (loadedScenes.TryGetValue(sceneAddress, out AsyncOperationHandle<SceneInstance> handle))
            {
                Addressables.UnloadSceneAsync(handle, true);
                loadedScenes.Remove(sceneAddress);
                Debug.Log($"Scene '{sceneAddress}' unloaded successfully.");
            }
            else
            {
                Debug.LogWarning($"Scene '{sceneAddress}' is not loaded, so it cannot be unloaded.");
            }
        }
    }
}
