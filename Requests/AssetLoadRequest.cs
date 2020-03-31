using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AssetBundleSimplified
{
    public class AssetLoadRequest<T> : CustomYieldInstruction where T : Object
    {
        public T[] AllAssets;

        public T Asset
        {
            get
            {
                return AllAssets != null && AllAssets.Length > 0 ? AllAssets[0] : null;
            }
        }

        public bool isAssetLoaded { get; private set; }

        private event Action<T> OnCompleteCallback;

        public event Action<T> OnComplete
        {
            add
            {
                if (isAssetLoaded)
                {
                    value(Asset);
                }
                else
                {
                    OnCompleteCallback += value;
                }
            }
            remove => OnCompleteCallback -= value;
        }

        public AssetLoadRequest(T[] assets)
        {
            AllAssets = assets;
            isAssetLoaded = true;
        }

        public AssetLoadRequest(T asset)
        {
            AllAssets = new[] {asset};
            isAssetLoaded = true;
        }

        public AssetLoadRequest(AssetBundleRequest createRequest)
        {
            createRequest.completed += LoadAssets;
        }

        public AssetLoadRequest(BundleLoadRequest createRequest)
        {
            createRequest.OnComplete += bundle =>
            {
                if (bundle == null)
                {
                    OnCompleteCallback?.Invoke(Asset);
                    return;
                }

                AssetBundleRequest request = bundle.LoadAllAssetsAsync<T>();
                request.completed += LoadAssets;
            };
        }

        public AssetLoadRequest(BundleLoadRequest createRequest, string assetKey)
        {
            createRequest.OnComplete += bundle =>
            {
                if (bundle == null)
                {
                    OnCompleteCallback?.Invoke(Asset);
                    return;
                }

                AssetBundleRequest request = bundle.LoadAssetAsync<T>(assetKey);
                request.completed += LoadAssets;
            };
        }

        public override bool keepWaiting => !IsLoaded();

        public AssetLoadRequestAwaiter<T> GetAwaiter()
        {
            return new AssetLoadRequestAwaiter<T>(this);
        }

        private void LoadAssets(AsyncOperation operation)
        {
            var request = operation as AssetBundleRequest;

            isAssetLoaded = true;
            AllAssets = new T[request.allAssets.Length];
            for (var i = 0; i < request.allAssets.Length; i++)
            {
                var asset = request.allAssets[i];
                AllAssets[i] = asset as T;
            }

            if (OnCompleteCallback != null)
            {
                var loadedAsset = Asset as T;
                OnCompleteCallback.Invoke(loadedAsset);
            }
        }

        private bool IsLoaded()
        {
            return isAssetLoaded;
        }
    }
}
