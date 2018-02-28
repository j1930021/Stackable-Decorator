using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StackableDecorator
{
    public class ColorAttribute : StackableDecoratorAttribute
    {
#if UNITY_EDITOR
        private Color m_Color;
        private Color m_GUIColor;
#endif
        public ColorAttribute(float r, float g, float b, float a)
        {
#if UNITY_EDITOR
            m_Color = new Color(r, g, b, a);
#endif
        }
#if UNITY_EDITOR
        public override bool BeforeGUI(ref Rect position, ref SerializedProperty property, ref GUIContent label, ref bool includeChildren, bool visible)
        {
            m_GUIColor = GUI.color;
            if (!IsVisible()) return visible;
            GUI.color = m_Color;
            return visible;
        }

        public override void AfterGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.color = m_GUIColor;
        }
#endif
    }
}