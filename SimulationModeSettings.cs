#if UNITY_EDITOR
using UnityEditor;

namespace AssetBundleSimplified
{
    public static class SimulationModeSettings
    {
        private const string SIMULATION_MODE_KEY = "Asset_Bundle_Simplified_Enable_Simulation";
        private const string ASSET_BUNDLE_MODE_MENU_NAME = "Tools/Asset Bundle Simplified/Simulation mode";
        
        [MenuItem(ASSET_BUNDLE_MODE_MENU_NAME)]
        public static void ToogleSimulationMode()
        {
            var currentMode = EditorPrefs.GetBool(SIMULATION_MODE_KEY, false);
            var newMode = !currentMode;

            SetSimulationMode(newMode);
        }
        
        [MenuItem(ASSET_BUNDLE_MODE_MENU_NAME, true)]
        private static bool UpdateSimulationModeToggle()
        {
            var currentMode = EditorPrefs.GetBool(SIMULATION_MODE_KEY, false);
            SetChecked(currentMode);
            return true;
        }
        
        public static bool IsSimulationModeEnabled()
        {
            return EditorPrefs.GetBool(SIMULATION_MODE_KEY);
        }

        public static void SetSimulationMode(bool newMode)
        {
            EditorPrefs.SetBool(SIMULATION_MODE_KEY, newMode);

            SetChecked(newMode);
        }

        private static void SetChecked(bool isEnabled)
        {
            Menu.SetChecked(ASSET_BUNDLE_MODE_MENU_NAME, isEnabled);
        }
    }
}
#endif