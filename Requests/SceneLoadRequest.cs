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
                if (isDone)
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
        
        private bool isDone;

        public SceneLoadRequest(BundleLoadRequest bundleLoadRequest, string sceneName, LoadSceneMode loadSceneMode)
        {
            bundleLoadRequest.OnComplete += assetBundle =>
            {
                var sceneRequest = SceneManager.LoadSceneAsync(sceneName, loadSceneMode);
                sceneRequest.completed += operation =>
                {
                    isDone = true;
                    if (onComplete != null)
                    {
                        onComplete();
                    }
                };
            };
        }

        public SceneLoadRequest(AsyncOperation loadSceneAsyncOp)
        {
            loadSceneAsyncOp.completed += operation =>
            {
                isDone = true;
                if (onComplete != null)
                {
                    onComplete();
                }
            };
        }

        public override bool keepWaiting
        {
            get
            {
                return !isDone;
            }
        }
    }
}