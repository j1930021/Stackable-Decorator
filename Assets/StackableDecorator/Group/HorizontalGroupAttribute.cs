using System.Linq;
using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StackableDecorator
{
    public class HorizontalGroupAttribute : StyledDecoratorAttribute
    {
        public bool indented = true;
        public bool prefix = false;
        public float spacing = 2;
        public bool fixedCell = true;
#if UNITY_EDITOR
        protected override string m_defaultStyle { get { return "label"; } }

        private string m_Name;
        private bool m_Children = false;
        private string[] m_Properties;
        private int m_PropertyCount;
        private float[] m_Widths;

        private Dictionary<string, Data> m_Data = new Dictionary<string, Data>();

        private class Data
        {
            public List<SerializedProperty> properties = new List<SerializedProperty>();
            public float height;
            public List<float> propertyHeights = new List<float>();
        }
#endif
        public HorizontalGroupAttribute(string name, bool children, string properties, params float[] widths)
        {
#if UNITY_EDITOR
            m_Name = name;
            m_Children = children;
            m_Properties = properties.Split(',');
            m_PropertyCount = 0;
            m_Widths = widths;
#endif
        }

        public HorizontalGroupAttribute(string name, int properties, params float[] widths)
        {
#if UNITY_EDITOR
            m_Name = name;
            m_Children = false;
            m_Properties = new string[] { };
            m_PropertyCount = properties;
            m_Widths = widths;
#endif
        }
#if UNITY_EDITOR
        private void GetProperties(List<SerializedProperty> list)
        {
            if (m_PropertyCount > 0)
                m_SerializedProperty.GetProperties(m_PropertyCount, list);
            else if (m_Children)
                m_SerializedProperty.GetChildrenProperties(m_Properties, list);
            else
                m_SerializedProperty.GetProperties(m_Properties, list);
        }

        public override float GetHeight(SerializedProperty property, GUIContent label, float height)
        {
            if (!IsVisible()) return height;

            var data = m_Data.Get(property.propertyPath);
            data.height = height;
            GetProperties(data.properties);
            data.propertyHeights.Clear();

            var widths = m_Widths.AsEnumerable();
            int diff = data.properties.Count + 1 - m_Widths.Length;
            if (diff > 0) widths = widths.Concat(Enumerable.Repeat(-1f, diff));
            widths = widths.Take(data.properties.Count + 1);

            int index = -1;
            InGroupAttribute.Add(m_Name);
            float result = 0;
            foreach (var width in widths)
            {
                if (index < 0 && width != 0)
                    result = Mathf.Max(data.height, result);
                if (index >= 0)
                {
                    height = data.properties[index] == null ? 0 : EditorGUI.GetPropertyHeight(data.properties[index], null, true);
                    if (height < 0) height = 0;
                    data.propertyHeights.Add(height);
                    if (width != 0)
                        result = Mathf.Max(height, result);
                }
                index++;
            }
            InGroupAttribute.Remove(m_Name);
            result = Mathf.Max(0, result);
            return result;
        }

        public override bool BeforeGUI(ref Rect position, ref SerializedProperty property, ref GUIContent label, ref bool includeChildren, bool visible)
        {
            if (!IsVisible()) return visible;
            if (!visible) return false;
            if (Event.current.type == EventType.Layout) return visible;

            var text = label.text;
            var image = label.image;
            var tooltip = label.tooltip;

            var content = m_Content;
            if (title == null)
                content.text = text;
            if (icon == null)
                content.image = image;
            if (tooltip == null)
                content.tooltip = tooltip;
            Rect rect;
            if (prefix)
                rect = EditorGUI.PrefixLabel(position, content, m_Style);
            else
                rect = indented ? EditorGUI.IndentedRect(position) : new Rect(position);

            var data = m_Data.Get(property.propertyPath);

            var widths = m_Widths.AsEnumerable();
            int diff = data.properties.Count + 1 - m_Widths.Length;
            if (diff > 0) widths = widths.Concat(Enumerable.Repeat(-1f, diff));
            widths = widths.Take(data.properties.Count + 1);

            int index = -1;
            var indentLevel = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            InGroupAttribute.Add(m_Name);
            foreach (var cell in rect.HorizontalDistribute(spacing, widths))
            {
                if (index < 0)
                    position = cell.Height(data.height);
                else if (data.properties[index] != null && cell.width > 0)
                {
                    var h = data.propertyHeights[index];
                    EditorGUI.PropertyField(cell.Height(h), data.properties[index], null, true);
                }
                index++;
            }
            InGroupAttribute.Remove(m_Name);
            EditorGUI.indentLevel = indentLevel;

            label.text = text;
            label.image = image;
            label.tooltip = tooltip;
            return position.width > 0;
        }
#endif
    }
}