using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StackableDecorator
{
    public class FoldoutAttribute : Styled2DecoratorAttribute
    {
        public bool indented = true;
        public bool hierarchyMode = true;
        public bool indentChildren = true;
#if UNITY_EDITOR
        protected override string m_defaultStyle { get { return "Foldout"; } }
        protected override string m_defaultStyle2 { get { return "label"; } }

        private int m_SavedIndentLevel;
#endif
        public FoldoutAttribute()
        {
#if UNITY_EDITOR
#endif
        }
#if UNITY_EDITOR
        public override float GetHeight(SerializedProperty property, GUIContent label, float height)
        {
            if (!IsVisible()) return height;
            height += EditorGUIUtility.singleLineHeight + 2;
            if (!property.isExpanded)
                height = EditorGUIUtility.singleLineHeight;
            return height;
        }

        public override bool BeforeGUI(ref Rect position, ref SerializedProperty property, ref GUIContent label, ref bool includeChildren, bool visible)
        {
            m_SavedIndentLevel = EditorGUI.indentLevel;
            if (!IsVisible()) return visible;
            if (!visible) return false;
            if (Event.current.type == EventType.Layout) return visible;

            var hierarchyMode = EditorGUIUtility.hierarchyMode;
            EditorGUIUtility.hierarchyMode = this.hierarchyMode;

            var h = EditorGUIUtility.singleLineHeight;
            var indent = position.indent(indented);
            GUI.Label(indent.Height(h), GUIContent.none, m_Style2);

            property.isExpanded = EditorGUI.Foldout(position.Height(h), property.isExpanded, m_Content, true, m_Style);

            position.yMin += h + 2;

            EditorGUIUtility.hierarchyMode = hierarchyMode;
            if (indentChildren && property.isExpanded)
                EditorGUI.indentLevel++;

            return property.isExpanded;
        }

        public override void AfterGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.indentLevel = m_SavedIndentLevel;
        }
#endif
    }
}