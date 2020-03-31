using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace AssetBundleSimplified
{
    public class BundleLoadRequest : CustomYieldInstruction
    {
        public AssetBundle Bundle;
        private bool isMainBundleLoaded;
        private Queue<AsyncOperation> dependenciesQueue;

        private Action<AssetBundle> onCompleteCallback;
        public event Action<AssetBundle> OnComplete
        {
            add
            {
                if (IsLoaded())
                {
                    value(Bundle);
                }
                else
                {
                    onCompleteCallback += value;
                }
            }
            remove => onCompleteCallback -= value;
        }


        public BundleLoadRequest(AssetBundle assetBundle)
        {
            Bundle = assetBundle;
            isMainBundleLoaded = true;
            dependenciesQueue = new Queue<AsyncOperation>();
        }

        public BundleLoadRequest(AssetBundleCreateRequest createRequest, List<AsyncOperation> dependencies)
        {
            createRequest.completed += operation =>
            {
                isMainBundleLoaded = true;
                Bundle = createRequest.assetBundle;
                UpdateDependencies();
            };

            dependenciesQueue = new Queue<AsyncOperation>(dependencies);
            foreach (var asyncOperation in dependenciesQueue)
            {
                asyncOperation.completed += operation =>
                {
                    UpdateDependencies();
                };
            }
        }

        public BundleLoadRequest(IBundleDownloadRequest downloadRequest)
        {
            dependenciesQueue = new Queue<AsyncOperation>();

            downloadRequest.Completed += () =>
            {
                isMainBundleLoaded = true;
                Bundle = downloadRequest.Bundle;

                if (IsLoaded())
                {
                    onCompleteCallback?.Invoke(Bundle);
                }
            };
        }

        public override bool keepWaiting
        {
            get
            {
                var isLoaded = IsLoaded();
                return !isLoaded;
            }
        }

        private void UpdateDependencies()
        {
            while (dependenciesQueue.Count > 0)
            {
                if (dependenciesQueue.Peek().isDone)
                {
                    dependenciesQueue.Dequeue();
                }
                else
                {
                    break;
                }
            }

            if (IsLoaded())
            {
                onCompleteCallback?.Invoke(Bundle);
            }
        }

        private bool IsLoaded()
        {
            return isMainBundleLoaded && dependenciesQueue.Count == 0;
        }
    }
}
