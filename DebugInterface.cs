using System.Collections.Generic;

namespace AssetBundleSimplified
{
    public class DebugInterface : IDebugInterface
    {
        private AssetBundleCache cache;
        
        public DebugInterface(AssetBundleCache cache)
        {
            this.cache = cache;
        }
        
        public Dictionary<string, int> GetCurrentReferenceCounters()
        {
            return cache.GetCurrentReferenceCounters();
        }

        public List<string> GetCurrentLoadedBundles()
        {
            return cache.GetLoadedBundleNames();
        }
    }
}