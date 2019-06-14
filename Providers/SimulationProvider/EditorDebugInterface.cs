using System.Collections.Generic;

namespace AssetBundleSimplified
{
    public class EditorDebugInterface : IDebugInterface
    {
        public Dictionary<string, int> GetCurrentReferenceCounters()
        {
            return new Dictionary<string, int>();
        }

        public List<string> GetCurrentLoadedBundles()
        {
            return new List<string>();
        }
    }
}