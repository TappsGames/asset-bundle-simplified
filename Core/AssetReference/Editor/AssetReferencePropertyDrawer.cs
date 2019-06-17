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

        private float propertyHeight = 0f;
        private float padding = 3f;
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {            
            EditorGUI.BeginProperty(position, label, property);
            
            position = EditorGUI.PrefixLabel(position,
                GUIUtility.GetControlID(FocusType.Passive),
                label);
            
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

            propertyHeight = padding;

            Rect objPosition = position;
            objPosition.y += propertyHeight;
            objPosition.height = 16.0f;
            asset = EditorGUI.ObjectField(objPosition, previousAsset, assetType, false);
            propertyHeight += objPosition.height;

            string assetPath = string.Empty;
            string bundleName = string.Empty;

                           
            if (asset != null)
            {
                assetPath = AssetDatabase.GetAssetPath(asset);
                bundleName = AssetDatabase.GetImplicitAssetBundleName(assetPath);
            }
            
            assetNameProperty.stringValue = asset != null ? asset.name : string.Empty;
            bundleNameProperty.stringValue = asset != null ? bundleName : string.Empty;

            if (!string.IsNullOrEmpty(bundleName) && !string.IsNullOrEmpty(assetPath))
            {
                return;
            }

            var messagePosition = position;
            messagePosition.y = position.y + propertyHeight;
            messagePosition.height = 32.0f;

            propertyHeight += messagePosition.height;
                        
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
            
            EditorGUI.EndProperty();
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
            return propertyHeight + padding;
        }
    }
}