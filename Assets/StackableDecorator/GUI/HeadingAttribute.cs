using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StackableDecorator
{
    public class HeadingAttribute : StyledDecoratorAttribute
    {
        public float width = -1;
        public float height = -1;
        public bool indented = true;
        public bool below = false;
        public TextAlignment alignment = TextAlignment.Left;
#if UNITY_EDITOR
        protected override string m_defaultStyle { get { return "BoldLabel"; } }

        private float m_Left;
        private float m_Right;
        private float m_Top;
        private float m_Bottom;
#endif
        public HeadingAttribute(float left = 0, float right = 0, float top = 0, float bottom = 0)
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
            float heading = this.height < 0 ? m_ContentSize.y : this.height;
            heading += m_Top + m_Bottom;
            if (heading < 0) heading = 0;
            return height + heading;
        }

        public override bool BeforeGUI(ref Rect position, ref SerializedProperty property, ref GUIContent label, ref bool includeChildren, bool visible)
        {
            if (!IsVisible()) return visible;
            if (!visible) return false;

            var indent = position.indent(indented);
            var rect = new Rect(indent);

            float width = 0;
            if (this.width < 0)
                width = m_ContentSize.x;
            else if (this.width <= 1)
                width = indent.width * this.width;
            else
                width = this.width;
            rect.width = width;
            rect.height = height < 0 ? m_ContentSize.y : height;

            if (alignment == TextAlignment.Left)
            {
                if (m_Left > 0) rect.x += m_Left;
            }
            else if (alignment == TextAlignment.Right)
            {
                rect.xMin = indent.xMax - rect.width;
                rect.xMax = indent.xMax;
                if (m_Right > 0) rect.x -= m_Right;
            }
            else if (alignment == TextAlignment.Center)
            {
                if (m_Left > 0) rect.width -= m_Left;
                if (m_Right > 0) rect.width -= m_Right;
                rect.x += (indent.width - rect.width) / 2;
            }
            if (m_Left < 0) rect.xMin += m_Left;
            if (m_Right < 0) rect.xMax -= m_Right;

            if (below)
                rect.y = indent.yMax - rect.height - m_Top - m_Bottom;
            rect.y += m_Top;

            GUI.Label(rect, m_Content, m_Style);

            float heading = rect.height + m_Top + m_Bottom;
            if (heading < 0) heading = 0;
            if (below)
                position.height -= heading;
            else
                position.yMin += heading;
            return true;
        }
#endif
    }
}