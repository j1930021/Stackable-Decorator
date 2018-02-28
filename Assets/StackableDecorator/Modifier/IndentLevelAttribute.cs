using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StackableDecorator
{
    public class IndentLevelAttribute : StackableDecoratorAttribute
    {
        public bool absolute = false;
#if UNITY_EDITOR
        private int m_IndentLevel;
        private int m_SavedIndentLevel;
#endif
        public IndentLevelAttribute(int indentLevel)
        {
#if UNITY_EDITOR
            m_IndentLevel = indentLevel;
#endif
        }
#if UNITY_EDITOR
        public override float GetHeight(SerializedProperty property, GUIContent label, float height)
        {
            return height;
        }

        public override bool BeforeGUI(ref Rect position, ref SerializedProperty property, ref GUIContent label, ref bool includeChildren, bool visible)
        {
            m_SavedIndentLevel = EditorGUI.indentLevel;
            if (!IsVisible()) return visible;
            EditorGUI.indentLevel = absolute ? m_IndentLevel : m_SavedIndentLevel + m_IndentLevel;
            return visible;
        }
        public override void AfterGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.indentLevel = m_SavedIndentLevel;
        }
#endif
    }
}