using System.IO;
using UnityEditor;
using UnityEngine;

namespace AssetBundleSimplified.Editor
{
    public static class AssetBundleBuilder
    {
        [MenuItem("Tools/Asset Bundle Simplified/Build Asset bundles")]
        public static void BuildAssetBundles()
        {
            string assetBundlesPath = Path.Combine(Application.streamingAssetsPath, BundleResources.PATH_IN_STREAMING_ASSETS);

            if (Directory.Exists(assetBundlesPath))
            {
                Directory.Delete(assetBundlesPath, true);
            }

            Directory.CreateDirectory(assetBundlesPath);

            BuildPipeline.BuildAssetBundles(assetBundlesPath,
                BuildAssetBundleOptions.ForceRebuildAssetBundle | BuildAssetBundleOptions.ChunkBasedCompression, 
                EditorUserBuildSettings.activeBuildTarget);

            AssetDatabase.Refresh();
        }
    }
}