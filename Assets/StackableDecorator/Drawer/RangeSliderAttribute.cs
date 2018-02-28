using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StackableDecorator
{
    public class RangeSliderAttribute : StackableFieldAttribute
    {
        public bool integer = false;
        public bool showInLabel = false;
#if UNITY_EDITOR
        private float m_Min;
        private float m_Max;
#endif
        public RangeSliderAttribute(float min, float max)
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
            Vector2 value;
#if UNITY_2017_2_OR_NEWER
            if (property.propertyType == SerializedPropertyType.Vector2Int)
            {
                integer = true;
                value = property.vector2IntValue;
            }
            else if (property.propertyType == SerializedPropertyType.Vector2)
#else
            if (property.propertyType == SerializedPropertyType.Vector2)
#endif
            {
                value = property.vector2Value;
            }
            else
            {
                EditorGUI.LabelField(position, label.text, "Use with Vector2 or Vector2Int.");
                return;
            }

            if (showInLabel)
            {
                label.text += " " + value.ToString(integer ? "0" : "0.00");
            }
            label = EditorGUI.BeginProperty(position, label, property);
            EditorGUI.MinMaxSlider(position, label, ref value.x, ref value.y, m_Min, m_Max);
            if (integer)
            {
                value.x = Mathf.RoundToInt(value.x);
                value.y = Mathf.RoundToInt(value.y);
            }
#if UNITY_2017_2_OR_NEWER
            if (property.propertyType == SerializedPropertyType.Vector2Int)
                if (Vector2Int.RoundToInt(value) != property.vector2IntValue)
                    property.vector2IntValue = Vector2Int.RoundToInt(value);
#endif
            if (property.propertyType == SerializedPropertyType.Vector2)
            {
                if (value != property.vector2Value)
                    property.vector2Value = value;
            }
            EditorGUI.EndProperty();
        }
#endif
    }
}