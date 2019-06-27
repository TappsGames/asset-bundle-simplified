using System;
using System.Runtime.CompilerServices;
using Object = UnityEngine.Object;

namespace AssetBundleSimplified
{
#if UNITY_2017
    public class AssetLoadRequestAwaiter<T> where T : Object
#else
    public class AssetLoadRequestAwaiter<T> : INotifyCompletion where T : Object
#endif
    
    
    {
        private readonly AssetLoadRequest<T> assetLoadRequest;

        public AssetLoadRequestAwaiter(AssetLoadRequest<T> assetLoadRequest)
        {
            this.assetLoadRequest = assetLoadRequest;
        }

        public bool IsCompleted()
        {
            return assetLoadRequest.isAssetLoaded;  
        } 

        public AssetLoadRequest<T> GetResult()
        {
            return assetLoadRequest;
        }

        public void OnCompleted(Action continuation)
        {
            if (assetLoadRequest.isAssetLoaded)
            {
                continuation();
            }
            else
            {
                assetLoadRequest.OnComplete += _ => { continuation(); };
            }
        }
    }
}