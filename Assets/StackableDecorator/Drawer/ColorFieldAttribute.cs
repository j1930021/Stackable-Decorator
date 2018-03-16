using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StackableDecorator
{
    public class ColorFieldAttribute : StackableFieldAttribute
    {
        public bool showEyedropper = true;
        public bool showAlpha = true;
        public bool hdr = false;
        public float minBrightness = 0f;
        public float maxBrightness = 8f;
        public float minExposureValue = 0.125f;
        public float maxExposureValue = 3f;
#if UNITY_EDITOR
        private ColorPickerHDRConfig m_HDRConfig = null;
#endif
        public ColorFieldAttribute()
        {
#if UNITY_EDITOR
#endif
        }
#if UNITY_EDITOR
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label, bool includeChildren)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label, bool includeChildren)
        {
            if (property.propertyType != SerializedPropertyType.Color)
            {
                EditorGUI.LabelField(position, label.text, "Use with Color.");
                return;
            }

            EditorGUI.BeginChangeCheck();
            Color colorValue;
#if UNITY_2018_1_OR_NEWER
            colorValue = EditorGUI.ColorField(position, label, property.colorValue, showEyedropper, showAlpha, hdr);
#else
            if (m_HDRConfig == null)
                m_HDRConfig = new ColorPickerHDRConfig(minBrightness, maxBrightness, minExposureValue, maxExposureValue);
            colorValue = EditorGUI.ColorField(position, label, property.colorValue, showEyedropper, showAlpha, hdr, m_HDRConfig);
#endif
            if (EditorGUI.EndChangeCheck())
                property.colorValue = colorValue;
        }
#endif
    }
}