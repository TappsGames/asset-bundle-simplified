using UnityEngine.Networking;

namespace AssetBundleSimplified
{
    public interface IRemoteBundleProvider
    {
        /// <summary>
        /// Returns true if the bundle with the provided name should be download from a remote server
        /// </summary>
        /// <param name="bundleName"></param>
        /// <returns></returns>
        bool IsRemoteBundle(string bundleName);

        /// <summary>
        /// Downloads a asset bundle from a remote server
        /// </summary>
        /// <param name="bundleName"></param>
        /// <returns></returns>
        IBundleDownloadRequest DownloadBundle(string bundleName);
    }
}