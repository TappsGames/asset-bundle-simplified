using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace AssetBundleSimplified
{
    public class BundleResourcesProvider : IBundleResourcesProvider
    {
        private static AssetBundleManifest bundlesManifest;
        private AssetBundleCache assetBundleCache;
        private DebugInterface debugInterface;
        private AssetBundle manifestBundle;
        private Dictionary<string, AssetBundleCreateRequest> currentAsyncBundleLoadOperations;

        public BundleResourcesProvider()
        {
            Init();
        }

        /// <summary>
        /// Unloads all currently loaded asset bundles
        /// </summary>
        public void UnloadAll()
        {
            manifestBundle.Unload(true);
            assetBundleCache.RemoveAllBundles();
            Init();
        }

        /// <summary>
        /// Unloads a bundle
        /// </summary>
        /// <param name="bundleName"></param>
        public void UnloadBundle(string bundleName)
        {
            Debug.Log($"Unloading bundle: {bundleName}");

            assetBundleCache.RemoveBundle(bundleName, false);
        }

        /// <summary>
        /// Check if bundle name is loaded.
        /// </summary>
        /// <param name="bundleName"></param>
        /// <returns></returns>
        public bool IsBundleLoaded(string bundleName)
        {
            return assetBundleCache.IsBundleLoaded(bundleName);
        }

        /// <summary>
        /// Loads all assets of a specific type on an asset bundle
        /// </summary>
        /// <param name="bundleName"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T[] LoadAllAssets<T>(string bundleName) where T : Object
        {
            AssetBundle bundle = LoadBundle(bundleName);
            Object[] assets = bundle.LoadAllAssets<T>();
            return (T[]) assets;
        }

        public AssetLoadRequest<T> LoadAllAssetsAsync<T>(string bundleName) where T : Object
        {
            var bundleLoadRequest = LoadBundleAsync(bundleName);
            return new AssetLoadRequest<T>(bundleLoadRequest);
        }

        /// <summary>
        /// Load an asset of a specific type on an asset bundle
        /// </summary>
        /// <param name="bundleName"></param>
        /// <param name="assetPath"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T LoadAsset<T>(string bundleName, string assetPath) where T: Object
        {
            AssetBundle bundle = LoadBundle(bundleName);
            Object asset = bundle.LoadAsset<T>(assetPath);
            return asset as T;
        }

        public AssetLoadRequest<T> LoadAssetAsync<T>(string bundleName, string assetKey) where T: Object
        {
            var bundleLoadRequest = LoadBundleAsync(bundleName);
            return new AssetLoadRequest<T>(bundleLoadRequest, assetKey);
        }

        /// <summary>
        /// Load an asset that has sub assets of a specific type  on an asset bundle
        /// </summary>
        /// <param name="bundleName"></param>
        /// <param name="assetPath"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T[] LoadAssetWithSubAssets<T>(string bundleName, string assetPath) where T : Object
        {
            AssetBundle bundle = LoadBundle(bundleName);
            Object[] asset = bundle.LoadAssetWithSubAssets<T>(assetPath);
            return asset as T[];
        }

        /// <summary>
        /// Loads a scene
        /// </summary>
        /// <param name="bundleName"></param>
        /// <param name="sceneName"></param>
        /// <param name="loadSceneMode"></param>
        /// <returns></returns>
        public SceneLoadRequest LoadScene(string bundleName, string sceneName, LoadSceneMode loadSceneMode)
        {
            var bundleRequest = LoadBundleAsync(bundleName);
            return new SceneLoadRequest(bundleRequest, sceneName, loadSceneMode);
        }

        /// <summary>
        /// Loads a scene
        /// </summary>
        /// <param name="bundleName"></param>
        /// <param name="sceneName"></param>
        /// <param name="loadSceneMode"></param>
        /// <returns></returns>
        public AsyncOperation UnloadScene(string bundleName, string sceneName)
        {
            var asyncOp =  SceneManager.UnloadSceneAsync(sceneName);

            asyncOp.completed += (operation) => {
                UnloadBundle(bundleName);
            };

            return asyncOp;
        }

        public IDebugInterface GetDebugInterface()
        {
            return debugInterface;
        }

        /// <summary>
        /// Load an asset bundle and its dependencies
        /// </summary>
        /// <param name="bundleName"></param>
        /// <returns></returns>
        public AssetBundle LoadBundle(string bundleName)
        {
            AssetBundle assetBundle;
            if (assetBundleCache.TryGetBundle(bundleName, out assetBundle))
            {
                return assetBundle;
            }

            Debug.Log($"Loading bundle: {bundleName}");
            assetBundle = LoadBundleFromFile(bundleName);
            assetBundleCache.AddBundle(bundleName, assetBundle, false);
            LoadBundleDependencies(bundleName);
            return assetBundle;
        }

        /// <summary>
        /// Asynchronously load an asset bundle and its dependencies
        /// </summary>
        /// <param name="bundleName"></param>
        /// <returns></returns>
        public BundleLoadRequest LoadBundleAsync(string bundleName)
        {
            AssetBundle loadedBundle;
            if (assetBundleCache.TryGetBundle(bundleName, out loadedBundle))
            {
                return new BundleLoadRequest(loadedBundle);
            }

            var request = LoadBundleFromFileAsync(bundleName, false);
            var dependencies = LoadBundleDependenciesAsync(bundleName);
            return new BundleLoadRequest(request, dependencies);
        }

        private AssetBundle LoadBundleFromFile(string bundleName)
        {
            var bundlesDirectory = Path.Combine(Application.streamingAssetsPath, BundleResources.PATH_IN_STREAMING_ASSETS);
            var bundlePath = Path.Combine(bundlesDirectory, bundleName);

            var assetBundle = AssetBundle.LoadFromFile(bundlePath);
            return assetBundle;
        }

        private AssetBundleCreateRequest LoadBundleFromFileAsync(string bundleName, bool isDependency)
        {
            AssetBundleCreateRequest request;
            if (currentAsyncBundleLoadOperations.TryGetValue(bundleName, out request))
            {
                return request;
            }

            var bundlesDirectory = Path.Combine(Application.streamingAssetsPath, BundleResources.PATH_IN_STREAMING_ASSETS);
            var bundlePath = Path.Combine(bundlesDirectory, bundleName);

            request = AssetBundle.LoadFromFileAsync(bundlePath);
            request.completed += operation =>
            {
                assetBundleCache.AddBundle(bundleName, request.assetBundle, isDependency);
                currentAsyncBundleLoadOperations.Remove(bundleName);
            };
            currentAsyncBundleLoadOperations.Add(bundleName, request);
            return request;
        }

        private List<AsyncOperation> LoadBundleDependenciesAsync(string bundleName)
        {
            var bundleDependencies = bundlesManifest.GetAllDependencies(bundleName);
            var bundleRequests = new List<AsyncOperation>();

            foreach (var bundleDependencyName in bundleDependencies)
            {
                if (assetBundleCache.IsBundleLoaded(bundleDependencyName))
                {
                    continue;
                }

                var request = LoadBundleFromFileAsync(bundleDependencyName, true);
                bundleRequests.Add(request);
            }

            return bundleRequests;
        }

        private void LoadBundleDependencies(string bundleName)
        {
            var bundleDependencies = bundlesManifest.GetAllDependencies(bundleName);

            foreach (var bundleDependencyName in bundleDependencies)
            {
                if (assetBundleCache.IsBundleLoaded(bundleDependencyName))
                {
                    continue;
                }

                var bundle = LoadBundleFromFile(bundleDependencyName);
                assetBundleCache.AddBundle(bundleDependencyName, bundle, true);
            }
        }

        private void UnloadBundleDependencies(string bundleName)
        {
            var bundleDependencies = bundlesManifest.GetAllDependencies(bundleName);

            foreach (var bundleDependencyName in bundleDependencies)
            {
                if (assetBundleCache.IsBundleLoaded(bundleDependencyName))
                {
                    continue;
                }

                assetBundleCache.RemoveBundle(bundleDependencyName, false);
            }
        }

        private void Init()
        {
            manifestBundle = LoadBundleFromFile("Bundles");
            bundlesManifest = manifestBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            Debug.Assert(bundlesManifest != null, "Cant load asset bundle manifest!");

            assetBundleCache = new AssetBundleCache(bundlesManifest);
            debugInterface = new DebugInterface(assetBundleCache);
            currentAsyncBundleLoadOperations = new Dictionary<string, AssetBundleCreateRequest>();
        }
    }
}