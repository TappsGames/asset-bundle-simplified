using System.Collections.Generic;
using UnityEditor;

namespace AssetBundleSimplified
{
    public class BundleResourcesDebugWindow : EditorWindow
    {
        private Dictionary<string, int> referenceCounters;
        private List<string> loadedBundles;

        [MenuItem("Tools/Asset Bundle Simplified/Debug Window")]
        public static void Init()
        {
            var window = GetWindow<BundleResourcesDebugWindow>();
            window.Show();
        }

        private void OnGUI()
        {
            if (!EditorApplication.isPlaying)
            {
                EditorGUILayout.HelpBox("Editor is not in Play Mode!", MessageType.Warning);
                return;
            }
            
            EditorGUILayout.LabelField("LoadedBundles", EditorStyles.boldLabel);
            foreach (var bundle in loadedBundles)
            {
                EditorGUILayout.LabelField(bundle);
            }
            
            EditorGUILayout.LabelField("Reference Counters", EditorStyles.boldLabel);
            foreach (var referenceCounter in referenceCounters)
            {
                EditorGUILayout.LabelField(string.Format("{0} : {1}", referenceCounter.Key, referenceCounter.Value));
            }
        }
        
        private void Update()
        {
            if (EditorApplication.isPlaying)
            {
                var debugInterface = BundleResources.GetDebugInterface();
                referenceCounters = debugInterface.GetCurrentReferenceCounters();
                loadedBundles = debugInterface.GetCurrentLoadedBundles();
                Repaint();
            }
        }

        private void Awake()
        {
            referenceCounters = new Dictionary<string, int>();
        } 
    }
}