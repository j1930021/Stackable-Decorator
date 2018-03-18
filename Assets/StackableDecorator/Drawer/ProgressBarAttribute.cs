using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StackableDecorator
{
    public class ProgressBarAttribute : StackableFieldAttribute
    {
        public bool prefix = false;
        public bool showLabel = true;
        public bool showPercentage = true;
        public int decimalPlaces = 1;
        public bool clampPercentage = true;
#if UNITY_EDITOR
        private float m_Min = 0;
        private float m_Max;
#endif
        public ProgressBarAttribute(float max)
        {
#if UNITY_EDITOR
            m_Max = max;
#endif
        }

        public ProgressBarAttribute(float min, float max)
        {
#if UNITY_EDITOR
            m_Min = min;
            m_Max = max;
#endif
        }
#if UNITY_EDITOR
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label, bool includeChildren)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label, bool includeChildren)
        {
            float value;
            switch (property.propertyType)
            {
                case SerializedPropertyType.Integer:
                    value = property.intValue;
                    break;
                case SerializedPropertyType.Float:
                    value = property.floatValue;
                    break;
                default:
                    EditorGUI.LabelField(position, label.text, "Use with float or int.");
                    return;
            }

            value = (value - m_Min) / (m_Max - m_Min);
            float pencent = value * 100;
            if (clampPercentage) pencent = Mathf.Clamp(pencent, 0, 100);
            string display = string.Empty;
            if (showLabel) display = label.text;
            if (showPercentage) display += " " + pencent.ToString("N" + decimalPlaces) + "%";

            label = EditorGUI.BeginProperty(position, label, property);
            if (prefix)
                position = EditorGUI.PrefixLabel(position, label);
            EditorGUI.ProgressBar(position, value, display);
            EditorGUI.EndProperty();
        }
#endif
    }
}