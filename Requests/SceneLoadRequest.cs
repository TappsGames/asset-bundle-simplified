using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AssetBundleSimplified
{
    public class SceneLoadRequest : CustomYieldInstruction
    {
        public event Action OnComplete
        {
            add
            {
                if (asyncOperation.isDone)
                {
                    value();
                    return;
                }

                onComplete += value;
            }
            remove
            {
                onComplete -= value;
            }
        }
        private Action onComplete;
        private AsyncOperation asyncOperation;

        public SceneLoadRequest(BundleLoadRequest bundleLoadRequest, string sceneName, LoadSceneMode loadSceneMode)
        {
            bundleLoadRequest.OnComplete += assetBundle =>
            {
                asyncOperation = SceneManager.LoadSceneAsync(sceneName, loadSceneMode);
                asyncOperation.completed += onSceneLoadCompleted;
            };
        }

        public SceneLoadRequest(AsyncOperation loadSceneAsyncOp)
        {
            asyncOperation = loadSceneAsyncOp;
            asyncOperation.completed += onSceneLoadCompleted;
        }

        private void onSceneLoadCompleted(AsyncOperation obj)
        {
            if (onComplete != null)
            {
                onComplete.Invoke();
            }
        }
        
        public override bool keepWaiting
        {
            get
            {
                if (asyncOperation == null)
                    return true; //Edge case when we have to wait for the asset bundle to load (bundleLoadRequest)
                else
                    return !asyncOperation.isDone;
            }
        }
    }
}