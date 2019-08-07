#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AssetBundleSimplified
{

    public class EditorBundleResourcesProvider : IBundleResourcesProvider
    {
        public LoadSceneParameters SceneLoadParams = new LoadSceneParameters();
        
        public void UnloadAll()
        {
        }

        public void UnloadBundle(string bundleName)
        {
        }

        public bool IsBundleLoaded(string bundleName)
        {
            return false;
        }

        public AssetBundle LoadBundle(string bundleName)
        {
            return null;
        }

        public BundleLoadRequest LoadBundleAsync(string bundleName)
        {
            return new BundleLoadRequest(LoadBundle(bundleName));
        }

        public T[] LoadAllAssets<T>(string bundleName) where T : Object
        {
            var assetKey = bundleName + typeof(T).FullName;
            Object[] assets;

            var assetsPath = AssetDatabase.GetAssetPathsFromAssetBundle(bundleName);
            var assetsList = new List<T>();

            foreach (var path in assetsPath)
            {
                var asset = AssetDatabase.LoadAssetAtPath<T>(path);

                if (asset != null)
                {
                    assetsList.Add(asset);
                }
            }

            assets = assetsList.ToArray();

            return (T[]) assets;
        }

        public AssetLoadRequest<T> LoadAllAssetsAsync<T>(string bundleName) where T : Object
        {
            var assets = LoadAllAssets<T>(bundleName);
            return new AssetLoadRequest<T>(assets);
        }

        public T LoadAsset<T>(string bundleName, string assetPath) where T : Object
        {
            var prefabIndex = assetPath.IndexOf(".prefab", StringComparison.InvariantCultureIgnoreCase);
            if (prefabIndex >= 0)
            {
                assetPath = assetPath.Substring(0, prefabIndex);
            }
            
            var path = AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(bundleName, assetPath);
            Object asset = AssetDatabase.LoadAssetAtPath<T>(path[0]);

            return asset as T;
        }

        public AssetLoadRequest<T> LoadAssetAsync<T>(string bundleName, string assetKey) where T : Object
        {
            var obj = LoadAsset<T>(bundleName, assetKey);
            return new AssetLoadRequest<T>(obj);
        }

        public SceneLoadRequest LoadScene(string bundleName, string sceneName, LoadSceneMode loadSceneMode)
        {
            var paths = AssetDatabase.GetAssetPathsFromAssetBundle(bundleName);
            var scene = Array.Find(paths, s => s.Contains(sceneName + ".unity"));

            AsyncOperation asyncOperation;
            if (loadSceneMode == LoadSceneMode.Single)
            {
                SceneLoadParams.loadSceneMode = LoadSceneMode.Single;
                asyncOperation = EditorSceneManager.LoadSceneAsyncInPlayMode(scene, SceneLoadParams);
            }
            else
            {
                SceneLoadParams.loadSceneMode = LoadSceneMode.Additive;
                asyncOperation = EditorSceneManager.LoadSceneAsyncInPlayMode(scene, SceneLoadParams);
            }
            return new SceneLoadRequest(asyncOperation);
        }

        public AsyncOperation UnloadScene(string bundleName, string sceneName)
        {
            return SceneManager.UnloadSceneAsync(sceneName);
        }

        public IDebugInterface GetDebugInterface()
        {
            return new EditorDebugInterface();
        }

        public void SetRemoteProvider(IRemoteBundleProvider remoteBundleProvider)
        {
        }
    }
}
#endif