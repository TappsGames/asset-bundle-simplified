using UnityEngine.SceneManagement;

namespace AssetBundleSimplified
{
    using UnityEngine;

    public static class BundleResources
    {
        public const string PATH_IN_STREAMING_ASSETS = "Bundles";

        private static IBundleResourcesProvider provider;

        static BundleResources()
        {
            provider = GetProvider();
        }

        public static void UnloadAll()
        {
            provider.UnloadAll();
        }

        public static bool IsBundleLoaded(string bundleName)
        {
            return provider.IsBundleLoaded(bundleName);
        }

        public static void UnloadBundle(string bundleName)
        {
            provider.UnloadBundle(bundleName);
        }
        
        public static void LoadBundle(string bundleName)
        {
            provider.LoadBundle(bundleName);
        }

        public static T[] LoadAllAssets<T>(string bundleName) where T : Object
        {
            return provider.LoadAllAssets<T>(bundleName);
        }

        public static AssetLoadRequest<T> LoadAllAssetsAsync<T>(string bundleName) where T : Object
        {
            return provider.LoadAllAssetsAsync<T>(bundleName);
        }

        public static T LoadAsset<T>(string bundleName, string assetPath) where T : Object
        {
            return provider.LoadAsset<T>(bundleName, assetPath);
        }
        
        public static AssetLoadRequest<T> LoadAssetAsync<T>(string assetBundle, string assetName) where T: Object
        {
            return provider.LoadAssetAsync<T>(assetBundle, assetName);
        }

        public static SceneLoadRequest LoadScene(string bundleName, string sceneName, LoadSceneMode loadMode)
        {
            return provider.LoadScene(bundleName, sceneName, loadMode);
        }
        
        public static AsyncOperation UnloadScene(string bundleName, string sceneName)
        {
            return provider.UnloadScene(bundleName, sceneName);
        }

        public static IDebugInterface GetDebugInterface()
        {
            return provider.GetDebugInterface();
        }

        
        private static IBundleResourcesProvider GetProvider()
        {
#if UNITY_EDITOR
            if (SimulationModeSettings.IsSimulationModeEnabled())
            {
                Debug.Log("[Asset bundle] Editor mode");
                return new EditorBundleResourcesProvider();
            }
#endif

            return new BundleResourcesProvider();
        }
    }
}