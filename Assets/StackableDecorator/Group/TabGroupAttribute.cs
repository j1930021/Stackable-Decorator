using System.Linq;
using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StackableDecorator
{
    public class TabGroupAttribute : Styled2DecoratorAttribute
    {
        public bool indented = true;
        public float width = -1;
        public string icons = string.Empty;
        public string tooltips = string.Empty;
        public TextAlignment alignment = TextAlignment.Center;
        public string buttonStyles = null;
#if UNITY_EDITOR
        protected override string m_defaultStyle { get { return "button"; } }
        protected override string m_defaultStyle2 { get { return "label"; } }

        private string m_Name;
        private string[] m_Tabs;
        private string[] m_Properties;

        private float m_Left;
        private float m_Right;
        private float m_Top;
        private float m_Bottom;

        private GUIContent[] m_Contents = null;
        private ButtonGroup m_ButtonGroup = null;
        private int m_TabIndex = 0;
        private Vector2 m_ToolbarSize = Vector2.zero;

        private Dictionary<string, Data> m_Data = new Dictionary<string, Data>();

        private class Data
        {
            public List<SerializedProperty> properties = new List<SerializedProperty>();
            public float tabHeight;
        }
#endif
        public TabGroupAttribute(string name, string tabs, string properties, float left = 0, float right = 0, float top = 0, float bottom = 0)
        {
#if UNITY_EDITOR
            m_Name = name;
            m_Tabs = tabs.Split(',');
            m_Properties = properties.Split(',');
            if (m_Tabs.Length != m_Properties.Length + 1)
            {
                m_Tabs = new string[] { "Tabs and properties not match." };
                m_Properties = new string[] { };
            }

            m_Left = left;
            m_Right = right;
            m_Top = top;
            m_Bottom = bottom;
#endif
        }
#if UNITY_EDITOR
        private void Update()
        {
            if (m_Contents == null)
            {
                m_ToolbarSize = Vector2.zero;
                m_Contents = m_Tabs.Select(tab => new GUIContent(tab)).ToArray();
                var icons = this.icons.Split(',');
                var tooltips = this.tooltips.Split(',');
                for (int i = 0; i < m_Contents.Length; i++)
                {
                    if (i < icons.Length && !string.IsNullOrEmpty(icons[i]))
                        m_Contents[i].image = icons[i].GetImage();
                    if (i < tooltips.Length)
                        m_Contents[i].tooltip = tooltips[i];

                    var size = m_Style.CalcSize(m_Contents[i]);
                    m_ToolbarSize.y = Mathf.Max(m_ToolbarSize.y, size.y);
                    m_ToolbarSize.x = Mathf.Max(m_ToolbarSize.x, size.x);
                }
                m_ToolbarSize.x *= m_Contents.Length;
            }

            if (m_ButtonGroup == null)
                m_ButtonGroup = new ButtonGroup(m_Contents, buttonStyles == null ? m_Style.name : buttonStyles);
        }

        public override float GetHeight(SerializedProperty property, GUIContent label, float height)
        {
            if (!IsVisible()) return height;

            Update();
            var data = m_Data.Get(property.propertyPath);
            property.GetProperties(m_Properties, data.properties);

            float toolbar = m_ToolbarSize.y;
            toolbar += m_Top + m_Bottom;
            if (toolbar < 0) toolbar = 0;

            float h = 0;
            if (m_TabIndex == 0)
                h = height;
            else
            {
                InGroupAttribute.Add(m_Name);
                if (data.properties[m_TabIndex - 1] != null)
                    h = EditorGUI.GetPropertyHeight(data.properties[m_TabIndex - 1], null, true);
                InGroupAttribute.Remove(m_Name);
            }
            data.tabHeight = h;

            return toolbar + h;
        }

        public override bool BeforeGUI(ref Rect position, ref SerializedProperty property, ref GUIContent label, ref bool includeChildren, bool visible)
        {
            if (!IsVisible()) return visible;
            if (!visible) return false;
            if (Event.current.type == EventType.Layout) return visible;

            Update();
            var data = m_Data.Get(property.propertyPath);
            property.GetProperties(m_Properties, data.properties);

            var indent = position.indent(indented);
            var rect = new Rect(indent);
            rect.xMin += m_Left;
            rect.xMax -= m_Right;
            rect.y += m_Top;
            rect.height = m_ToolbarSize.y;
            GUI.Label(rect, GUIContent.none, m_Style2);

            rect = new Rect(indent);
            float width = 0;
            if (this.width < 0)
                width = m_ToolbarSize.x;
            else if (this.width <= 1)
                width = indent.width * this.width;
            else
                width = this.width;
            rect.width = width;
            rect.y += m_Top;
            rect.height = m_ToolbarSize.y;

            if (alignment == TextAlignment.Right)
            {
                rect.xMin = indent.xMax - rect.width;
                rect.xMax = indent.xMax;
            }
            else if (alignment == TextAlignment.Center)
            {
                rect.x += (indent.width - rect.width) / 2;
            }
            rect.xMin += m_Left;
            rect.xMax -= m_Right;

            var tab = (int)m_ButtonGroup.Draw(rect, m_TabIndex, m_Contents.Length);

            float toolbar = m_ToolbarSize.y + m_Top + m_Bottom;
            if (toolbar < 0) toolbar = 0;
            position.yMin += toolbar;

            if (m_TabIndex > 0)
            {
                InGroupAttribute.Add(m_Name);
                if (data.properties[m_TabIndex - 1] != null)
                    EditorGUI.PropertyField(position.Height(data.tabHeight), data.properties[m_TabIndex - 1], null, true);
                InGroupAttribute.Remove(m_Name);
            }

            var result = m_TabIndex == 0;
            m_TabIndex = tab;
            return result;
        }
#endif
    }
}