using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StackableDecorator
{
    public class InlinePropertyAttribute : StyledDecoratorAttribute, INoCacheInspectorGUI
    {
        public bool indented = true;
        public bool always = false;
        public bool indentChildren = true;
        public float maxHeight = -1;

        public string maxHeightGetter = null;
#if UNITY_EDITOR
        protected override string m_defaultStyle { get { return "box"; } }

        private float m_Left;
        private float m_Right;
        private float m_Top;
        private float m_Bottom;

        private bool m_Visible;
        private Rect m_Position;

        private DynamicValue<float> m_DynamicMaxHeight = null;
        private Dictionary<string, Data> m_Data = new Dictionary<string, Data>();

        private class Data
        {
            public float height;
            public float inlineHeight;
            public float maxHeight;
            public SerializedObject serializedObject = null;
            public List<float> propertyHeights = new List<float>();
            public Vector2 scroll;
        }
#endif
        public InlinePropertyAttribute(float left = 3, float right = 3, float top = 3, float bottom = 3)
        {
#if UNITY_EDITOR
            m_Left = left;
            m_Right = right;
            m_Top = Mathf.Max(0, top);
            m_Bottom = Mathf.Max(0, bottom);
#endif
        }
#if UNITY_EDITOR
        private void Update()
        {
            if (m_DynamicMaxHeight == null)
                m_DynamicMaxHeight = new DynamicValue<float>(maxHeightGetter, m_SerializedProperty, maxHeight);
            m_DynamicMaxHeight.Update(m_SerializedProperty);
            if (maxHeightGetter != null)
                maxHeight = m_DynamicMaxHeight.GetValue();
        }

        public override float GetHeight(SerializedProperty property, GUIContent label, float height)
        {
            if (!IsVisible()) return height;
            if (property.propertyType != SerializedPropertyType.ObjectReference) return height;
            var data = m_Data.Get(property.propertyPath);
            if (property.objectReferenceValue == null)
            {
                data.serializedObject = null;
                return height;
            }
            if (!always && !property.isExpanded) return height;

            Update();
            data.height = height;
            data.maxHeight = maxHeight;
            data.serializedObject = new SerializedObject(property.objectReferenceValue);
            data.serializedObject.Update();

            var result = InlineProperty.GetHeight(data.serializedObject, data.propertyHeights);
            data.inlineHeight = result;

            if (maxHeight >= 0)
                result = Mathf.Min(result, maxHeight);
            height += result + 2 + m_Top + m_Bottom;
            if (result <= 0) height = data.height;
            return height;
        }

        public override bool BeforeGUI(ref Rect position, ref SerializedProperty property, ref GUIContent label, ref bool includeChildren, bool visible)
        {
            m_Visible = visible;
            if (!IsVisible()) return visible;
            if (!visible) return false;
            if (property.propertyType != SerializedPropertyType.ObjectReference) return visible;
            var data = m_Data.Get(property.propertyPath);
            if (data.serializedObject == null) return visible;
            if (!always && !property.isExpanded) return visible;

            m_Position = new Rect(position);
            position.height = data.height;
            return true;
        }

        public override void AfterGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!m_Visible) return;
            if (property.propertyType != SerializedPropertyType.ObjectReference) return;
            var data = m_Data.Get(property.propertyPath);
            if (data.serializedObject == null) return;
            if (!always && !property.isExpanded) return;

            var indent = m_Position.indent(indented);
            indent.yMin += data.height + 2;
            indent.yMax -= 0;
            if (m_Left < 0) indent.xMin += m_Left;
            if (m_Right < 0) indent.xMax -= m_Right;
            if (data.inlineHeight > 0)
                GUI.Label(indent, GUIContent.none, m_Style);

            var text = label.text;
            var image = label.image;
            var tooltip = label.tooltip;
            int indentLevel = EditorGUI.indentLevel;

            if (indentChildren) EditorGUI.indentLevel++;
            var rect = m_Position.indent(indentChildren);
            rect.yMin += data.height + 2;
            rect.xMin += Mathf.Max(0, m_Left);
            rect.xMax -= Mathf.Max(0, m_Right);
            rect.yMin += m_Top;
            EditorGUI.indentLevel = indentLevel;

            var realHeight = data.inlineHeight;
            var view = new Rect(rect);
            view.height = realHeight;
            if (realHeight > rect.height)
                view.width -= 16;
            var realWidth = view.width;

            if (realHeight > rect.height)
                data.scroll = GUI.BeginScrollView(rect, data.scroll, view);
            rect.width = realWidth;
            InlineProperty.Draw(rect, data.serializedObject, false, data.propertyHeights);
            if (realHeight > rect.height)
                GUI.EndScrollView();

            EditorGUI.indentLevel = indentLevel;
            label.text = text;
            label.image = image;
            label.tooltip = tooltip;
        }
#endif
    }
}