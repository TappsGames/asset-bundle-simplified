using UnityEditor;
using UnityEngine;

namespace AssetBundleSimplified.Editor
{
    public class CompressionSelector : EditorWindow
    {
        private static BuildAssetBundleOptions compressionType;
    
        [MenuItem("Tools/Asset Bundle Simplified/Compression config")]
        public static void BuildCompressionPopup()
        {
            compressionType = BuildAssetBundleOptions.ChunkBasedCompression;
            var window = GetWindow<CompressionSelector>();
            window.Show();
        }

        private void OnGUI()
        {
            compressionType = (BuildAssetBundleOptions)EditorGUILayout.EnumPopup("Asset bundle compression:", compressionType);
            
            if (GUILayout.Button("Save"))
            {
                EditorPrefs.SetString("COMPRESSION_TYPE", compressionType.ToString());
            }
            else if (GUILayout.Button("Save and build"))
            {
                EditorPrefs.SetString("COMPRESSION_TYPE", compressionType.ToString());
            
                AssetBundleBuilder.BuildAssetBundles();
            }
        }
    }
}
