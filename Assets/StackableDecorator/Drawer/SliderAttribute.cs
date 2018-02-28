using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StackableDecorator
{
    public class SliderAttribute : StackableFieldAttribute
    {
        public bool showField = true;
#if UNITY_EDITOR
        private float m_Min;
        private float m_Max;
#endif
        public SliderAttribute(float min, float max)
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
            if (property.propertyType != SerializedPropertyType.Float && property.propertyType != SerializedPropertyType.Integer)
            {
                EditorGUI.LabelField(position, label.text, "Use with float or int.");
                return;
            }

            if(showField)
            {
                if (property.propertyType == SerializedPropertyType.Float)
                    EditorGUI.Slider(position, property, m_Min, m_Max, label);
                if (property.propertyType == SerializedPropertyType.Integer)
                    EditorGUI.IntSlider(position, property, (int)m_Min, (int)m_Max, label);
                return;
            }

            label = EditorGUI.BeginProperty(position, label, property);
            if (property.propertyType == SerializedPropertyType.Float)
            {
                var value = GUI.HorizontalSlider(position, property.floatValue, m_Min, m_Max);
                if (value != property.floatValue)
                    property.floatValue = value;
            }
            if (property.propertyType == SerializedPropertyType.Integer)
            {
                var value = Mathf.RoundToInt(GUI.HorizontalSlider(position, property.intValue, m_Min, m_Max));
                if (value != property.intValue)
                    property.intValue = value;
            }
            EditorGUI.EndProperty();
        }
#endif
    }
}