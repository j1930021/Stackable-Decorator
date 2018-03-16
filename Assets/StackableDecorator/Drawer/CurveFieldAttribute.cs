using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StackableDecorator
{
    public class CurveFieldAttribute : StackableFieldAttribute
    {
#if UNITY_EDITOR
        private Color m_Color = Color.green;
        private Rect m_Range = new Rect();
#endif
        public CurveFieldAttribute()
        {
#if UNITY_EDITOR
#endif
        }

        public CurveFieldAttribute(float r, float g, float b)
        {
#if UNITY_EDITOR
            m_Color = new Color(r, g, b, 1);
#endif
        }

        public CurveFieldAttribute(float xMin, float yMin, float xMax, float yMax)
        {
#if UNITY_EDITOR
            m_Range = new Rect(xMin, yMin, xMax - xMin, yMax - yMin);
#endif
        }

        public CurveFieldAttribute(float r, float g, float b, float xMin, float yMin, float xMax, float yMax)
        {
#if UNITY_EDITOR
            m_Color = new Color(r, g, b, 1);
            m_Range = new Rect(xMin, yMin, xMax - xMin, yMax - yMin);
#endif
        }
#if UNITY_EDITOR
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label, bool includeChildren)
        {
            if (property.propertyType != SerializedPropertyType.AnimationCurve)
            {
                EditorGUI.LabelField(position, label.text, "Use with AnimationCurve.");
                return;
            }

            EditorGUI.CurveField(position, property, m_Color, m_Range, label);
        }
#endif
    }
}