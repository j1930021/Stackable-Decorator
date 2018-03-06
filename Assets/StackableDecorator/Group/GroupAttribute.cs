using UnityEngine;
using System.Linq;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StackableDecorator
{
    public class GroupAttribute : StackableDecoratorAttribute
    {
        public bool indented = false;
        public bool indentChildren = false;
        public float spacing = 2;
        public bool fixedCell = true;
#if UNITY_EDITOR
        private string m_Name;
        private bool m_Children = false;
        private string[] m_Properties;
        private int m_PropertyCount;
        private float[] m_Heights;
        private int m_SavedIndentLevel;

        private Dictionary<string, Data> m_Data = new Dictionary<string, Data>();

        private class Data
        {
            public List<SerializedProperty> properties = new List<SerializedProperty>();
            public List<float> propertyHeights = new List<float>();
            public List<float> rowHeights = new List<float>();
        }
#endif
        public GroupAttribute(string name, bool children, string properties, params float[] heights)
        {
#if UNITY_EDITOR
            m_Name = name;
            m_Children = children;
            m_Properties = properties.Split(',');
            m_PropertyCount = 0;
            m_Heights = heights;
#endif
        }

        public GroupAttribute(string name, int properties, params float[] heights)
        {
#if UNITY_EDITOR
            m_Name = name;
            m_Children = false;
            m_Properties = new string[] { };
            m_PropertyCount = properties;
            m_Heights = heights;
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
            GetProperties(data.properties);
            data.propertyHeights.Clear();
            data.rowHeights.Clear();

            var heights = m_Heights.AsEnumerable();
            int diff = data.properties.Count + 1 - m_Heights.Length;
            if (diff > 0) heights = heights.Concat(Enumerable.Repeat(-1f, diff));
            heights = heights.Take(data.properties.Count + 1);

            int index = -1;
            float result = 0;
            InGroupAttribute.Add(m_Name);
            foreach (var h in heights)
            {

                if (index < 0)
                    height = h >= 0 ? h : height;
                else
                {
                    var propertyHeight = data.properties[index] == null ? 0 : EditorGUI.GetPropertyHeight(data.properties[index], null, true);
                    data.propertyHeights.Add(propertyHeight);

                    if (h >= 0)
                        height = h;
                    else
                        height = propertyHeight;
                }

                if (height < 0) height = 0;
                data.rowHeights.Add(height);
                if (result > 0 && height > 0) result += spacing;
                result += height;
                index++;
            }
            InGroupAttribute.Remove(m_Name);
            return result;
        }

        public override bool BeforeGUI(ref Rect position, ref SerializedProperty property, ref GUIContent label, ref bool includeChildren, bool visible)
        {
            m_SavedIndentLevel = EditorGUI.indentLevel;
            if (!IsVisible()) return visible;
            if (!visible) return false;
            if (Event.current.type == EventType.Layout) return visible;

            var text = label.text;
            var image = label.image;
            var tooltip = label.tooltip;

            var rect = position.indent(indented);

            var data = m_Data.Get(property.propertyPath);
            GetProperties(data.properties);

            int index = -1;
            if (indentChildren)
                EditorGUI.indentLevel++;
            InGroupAttribute.Add(m_Name);
            foreach (var cell in rect.VerticalDistribute(spacing, data.rowHeights))
            {
                if (index < 0)
                    position = cell;
                else if (data.properties[index] != null && cell.height > 0)
                    EditorGUI.PropertyField(cell.Height(data.propertyHeights[index]), data.properties[index], null, true);
                index++;
            }
            InGroupAttribute.Remove(m_Name);

            label.text = text;
            label.image = image;
            label.tooltip = tooltip;
            return position.height > 0;
        }

        public override void AfterGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.indentLevel = m_SavedIndentLevel;
        }
#endif
    }
}