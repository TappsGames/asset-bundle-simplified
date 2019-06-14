using System.Collections.Generic;

namespace AssetBundleSimplified
{
    public interface IDebugInterface
    {
        Dictionary<string, int> GetCurrentReferenceCounters();
        List<string> GetCurrentLoadedBundles();
    }
}