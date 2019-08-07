using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AssetBundleSimplified
{
    public interface IBundleResourcesProvider
    {
        void UnloadAll();
        void UnloadBundle(string bundleName);
        bool IsBundleLoaded(string bundleName);
        AssetBundle LoadBundle(string bundleName);
        BundleLoadRequest LoadBundleAsync(string bundleName);
        T LoadAsset<T>(string bundleName, string assetPath) where T: Object;
        T[] LoadAllAssets<T>(string bundleName) where T : Object;
        AssetLoadRequest<T> LoadAssetAsync<T>(string bundleName, string assetKey) where T: Object;
        AssetLoadRequest<T> LoadAllAssetsAsync<T>(string bundleName) where T : Object;
        SceneLoadRequest LoadScene(string bundleName, string sceneName, LoadSceneMode loadSceneMode);
        AsyncOperation UnloadScene(string bundleName, string sceneName);
        IDebugInterface GetDebugInterface();

        void SetRemoteProvider(IRemoteBundleProvider remoteBundleProvider);
    }
}