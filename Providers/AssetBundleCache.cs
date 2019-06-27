using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AssetBundleSimplified
{
    public class AssetBundleCache
    {     
        private Dictionary<string, AssetBundle> loadedBundles = new Dictionary<string, AssetBundle>();
        private Dictionary<string, int> referenceCounters = new Dictionary<string, int>();
        private AssetBundleManifest manifest;

        public AssetBundleCache(AssetBundleManifest manifest)
        {
            this.manifest = manifest;
        }

        public bool TryGetBundle(string bundleName, out AssetBundle bundle)
        {
            return loadedBundles.TryGetValue(bundleName, out bundle);
        }

        public bool IsBundleLoaded(string bundleName)
        {
            return loadedBundles.ContainsKey(bundleName);
        }

        /// <summary>
        /// Adds an asset bundle to cache
        /// </summary>
        /// <param name="bundleName">Asset Bundle Name</param>
        /// <param name="bundle">Asset Bundle Instance</param>
        public void AddBundle(string bundleName, AssetBundle bundle, bool isDependency)
        {
            if (loadedBundles.ContainsKey(bundleName))
            {
                Debug.LogError("Can not load two bundles with the same name!");
                return;
            }
            
            loadedBundles.Add(bundleName, bundle);

            if (!isDependency)
            {
                AddReferenceToBundle(bundleName);
            }
            
            var dependencies = manifest.GetAllDependencies(bundleName);
            foreach (var dependency in dependencies)
            {
                AddReferenceToBundle(dependency);
            }
        }

        /// <summary>
        /// Unload a asset bundle
        /// </summary>
        /// <param name="bundleName">Asset Bundle Name</param>
        public void RemoveBundle(string bundleName, bool ignoreReferenceCount)
        {
            if (!loadedBundles.ContainsKey(bundleName))
            {
#if UNITY_2017
                Debug.LogWarning(string.Format("{0} is already unloaded!", bundleName));
#else
                Debug.LogWarning($"{bundleName} is already unloaded!");
#endif
                
                return;
            }

            var dependencies = manifest.GetAllDependencies(bundleName);
            foreach (var dependency in dependencies)
            {
                RemoveReferenceToBundle(dependency);
            }

            bool shouldRemove = ignoreReferenceCount;
            
            if (referenceCounters.ContainsKey(bundleName))
            {
                referenceCounters[bundleName] = referenceCounters[bundleName] - 1;
                
                if (referenceCounters[bundleName] <= 0 || ignoreReferenceCount)
                {
                    referenceCounters.Remove(bundleName);
                    shouldRemove = true;
                }
            }

            if (shouldRemove)
            {
                var bundle = loadedBundles[bundleName];
                bundle.Unload(true);
                loadedBundles.Remove(bundleName);   
            }
            
        }

        public void RemoveAllBundles()
        {
            foreach (var loadedBundle in loadedBundles.Values)
            {
                loadedBundle.Unload(true);
            }
            
            loadedBundles.Clear();
            referenceCounters.Clear();
        }

        public Dictionary<string, int> GetCurrentReferenceCounters()
        {
            return new Dictionary<string, int>(referenceCounters);
        }
        
        public List<string> GetLoadedBundleNames()
        {
            return loadedBundles.Keys.ToList();
        }

        private void AddReferenceToBundle(string bundleName)
        {
            if (referenceCounters.ContainsKey(bundleName))
            {
                referenceCounters[bundleName] = referenceCounters[bundleName] + 1;
            }
            else
            {
                referenceCounters.Add(bundleName, 1);
            }
        }

        private void RemoveReferenceToBundle(string bundleName)
        {
            if (!referenceCounters.ContainsKey(bundleName))
            {
#if UNITY_2017
                Debug.LogError(string.Format("There isn't a reference counter for {0}", bundleName));
#else
                Debug.LogError($"There isn't a reference counter for {bundleName}");
#endif
                
                return;
            }

            var count = referenceCounters[bundleName] - 1;
            referenceCounters[bundleName] = count;

            if (count <= 0)
            {
                RemoveBundle(bundleName, false);
            }
        }
    }
}