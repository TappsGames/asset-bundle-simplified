using System;
using System.Security.AccessControl;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AssetBundleSimplified.AssetReference
{
    //HACK: Generic classes are not serializable by Unity
    [System.Serializable]
    public abstract class AssetReference
    {
    }
    
    public abstract class AssetReference<T> : AssetReference where T : Object
    {
        public string AssetName;
        public string BundleName;
        
        public T Load()
        {
            return BundleResources.LoadAsset<T>(BundleName, AssetName);
        }

        public AssetLoadRequest<T> LoadAsync()
        {
            return BundleResources.LoadAssetAsync<T>(BundleName, AssetName);
        }
    }
    
    // HACK: As generic types are not serializable by Unity, we manually define non-generic types
    
    [Serializable] public class SpriteReference : AssetReference<Sprite>{}
    [Serializable] public class GameObjectReference : AssetReference<GameObject>{}
    [Serializable] public class ScriptableObjectReference : AssetReference<ScriptableObject>{}
    
}