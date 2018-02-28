using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StackableDecorator
{
    public class BoxAttribute : StyledDecoratorAttribute
    {
        public bool indented = true;
#if UNITY_EDITOR
        protected override string m_defaultStyle { get { return "box"; } }

        private float m_Left;
        private float m_Right;
        private float m_Top;
        private float m_Bottom;
#endif
        public BoxAttribute(float left = 0, float right = 0, float top = 0, float bottom = 0)
        {
#if UNITY_EDITOR
            m_Left = left;
            m_Right = right;
            m_Top = top;
            m_Bottom = bottom;
#endif
        }
#if UNITY_EDITOR
        public override float GetHeight(SerializedProperty property, GUIContent label, float height)
        {
            if (!IsVisible()) return height;
            var top = Mathf.Max(0, m_Top);
            var bottom = Mathf.Max(0, m_Bottom);
            return height + top + bottom;
        }

        public override bool BeforeGUI(ref Rect position, ref SerializedProperty property, ref GUIContent label, ref bool includeChildren, bool visible)
        {
            if (!IsVisible()) return visible;
            if (!visible) return false;

            var indent = position.indent(indented);
            if (m_Left < 0) indent.xMin += m_Left;
            if (m_Right < 0) indent.xMax -= m_Right;
            if (m_Top < 0) indent.yMin += m_Top;
            if (m_Bottom < 0) indent.yMax -= m_Bottom;
            GUI.Box(indent, GUIContent.none, m_Style);

            position.xMin += Mathf.Max(0, m_Left);
            position.xMax -= Mathf.Max(0, m_Right);
            position.yMin += Mathf.Max(0, m_Top);
            position.yMax -= Mathf.Max(0, m_Bottom);
            return true;
        }
#endif
    }
}