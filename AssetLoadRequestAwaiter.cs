using System;
using System.Runtime.CompilerServices;
using Object = UnityEngine.Object;

namespace AssetBundleSimplified
{
    public class AssetLoadRequestAwaiter<T> : INotifyCompletion where T : Object
    {
        private readonly AssetLoadRequest<T> assetLoadRequest;

        public AssetLoadRequestAwaiter(AssetLoadRequest<T> assetLoadRequest)
        {
            this.assetLoadRequest = assetLoadRequest;
        }

        public bool IsCompleted => assetLoadRequest.isAssetLoaded;

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