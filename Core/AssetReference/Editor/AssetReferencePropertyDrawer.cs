using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AssetBundleSimplified.AssetReference.Editor
{
    [CustomPropertyDrawer(typeof(AssetReference), true)]
    public class AssetReferencePropertyDrawer : PropertyDrawer
    {
        private Object asset;

        private int propertyHeight = 1;
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {            
            var assetNameProperty = property.FindPropertyRelative("AssetName");
            var bundleNameProperty = property.FindPropertyRelative("BundleName");

            Type propertyType = fieldInfo.FieldType;;
            Type baseType = propertyType.BaseType;

            Type assetType = baseType.GetGenericArguments()[0];

            Object previousAsset;
            if (asset == null)
            {
                previousAsset = LoadAsset(assetNameProperty, bundleNameProperty, assetType);
            }
            else
            {
                previousAsset = asset;
            }

            Rect objPosition = position;
            objPosition.height = 0.5f;
            asset = EditorGUI.ObjectField(position, property.name, previousAsset, assetType, false);

            string assetPath = string.Empty;
            string bundleName = string.Empty;

            if (previousAsset != asset)
            {                
                if (asset != null)
                {
                    assetPath = AssetDatabase.GetAssetPath(asset);
                    bundleName = AssetDatabase.GetImplicitAssetBundleName(assetPath);
                }
                
                assetNameProperty.stringValue = asset != null ? asset.name : string.Empty;
                bundleNameProperty.stringValue = asset != null ? bundleName : string.Empty;
            }

            if (!string.IsNullOrEmpty(bundleName))
            {
                propertyHeight = 1;
                return;
            }

            propertyHeight = 2;

            var messagePosition = position;
            messagePosition.y = -16.0f;
            messagePosition.height = 0.5f;
                        
            if (asset == null)
            {
                EditorGUI.HelpBox(messagePosition, "No asset is selected!", MessageType.Warning);
                return;
            }
            
            if (string.IsNullOrEmpty(bundleName))
            {
                EditorGUI.HelpBox(messagePosition, "The selected asset is not on a asset bundle, this reference will not work!",
                    MessageType.Error);
            }
        }

        private static Object LoadAsset(SerializedProperty assetNameProperty, SerializedProperty bundleNameProperty,
            Type assetType)
        {
            string assetPath =
                AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(
                    bundleNameProperty.stringValue, assetNameProperty.stringValue).FirstOrDefault();
            var asset = AssetDatabase.LoadAssetAtPath(assetPath, assetType);
            return asset;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label) * propertyHeight;
        }
    }
}