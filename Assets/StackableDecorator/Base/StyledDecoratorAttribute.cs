using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StackableDecorator
{
    public abstract class StyledDecoratorAttribute : StackableDecoratorAttribute
    {
        public string style = null;
        public string title = null;
        public string icon = null;
        public string tooltip = null;
#if UNITY_EDITOR
        private class DynamicContent
        {
            public DynamicValue<string> Title;
            public DynamicValue<string> Icon;
            public DynamicValue<string> Tooltip;
        }

        protected GUIStyle m_Style { get { return GetStyle(); } }
        protected GUIContent m_Content { get { return GetContent(); } }
        protected Vector2 m_ContentSize { get { return GetContentSize(); } }

        protected abstract string m_defaultStyle { get; }

        private GUIStyle m_StyleInternal = null;
        private GUIContent m_ContentInternal = null;
        private DynamicContent m_DynamicContent = null;

        private GUIStyle GetStyle()
        {
            if (m_StyleInternal == null)
                m_StyleInternal = style == null ? m_defaultStyle : style;
            return m_StyleInternal;
        }

        private GUIContent GetContent()
        {
            if (m_ContentInternal == null)
                m_ContentInternal = new GUIContent();

            if (m_DynamicContent == null)
            {
                m_DynamicContent = new DynamicContent();
                m_DynamicContent.Title = new DynamicValue<string>(title == null ? string.Empty : title, m_SerializedProperty);
                m_DynamicContent.Icon = new DynamicValue<string>(icon == null ? string.Empty : icon, m_SerializedProperty);
                m_DynamicContent.Tooltip = new DynamicValue<string>(tooltip == null ? string.Empty : tooltip, m_SerializedProperty);
            }
            m_DynamicContent.Title.Update(m_SerializedProperty);
            m_DynamicContent.Icon.Update(m_SerializedProperty);
            m_DynamicContent.Tooltip.Update(m_SerializedProperty);

            m_ContentInternal.text = m_DynamicContent.Title.GetValue();
            m_ContentInternal.tooltip = m_DynamicContent.Tooltip.GetValue();
            var image = m_DynamicContent.Icon.GetValue();
            if (!string.IsNullOrEmpty(image))
                m_ContentInternal.image = image.GetImage();
            return m_ContentInternal;
        }

        private Vector2 GetContentSize()
        {
            var size = new Vector2(0, 0);
            if (m_Content.text != string.Empty || m_Content.image != null)
                size = m_Style.CalcSize(m_Content);
            return size;
        }
#endif
    }
}