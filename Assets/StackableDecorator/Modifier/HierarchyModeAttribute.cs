using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StackableDecorator
{
    public class HierarchyModeAttribute : StackableDecoratorAttribute
    {
#if UNITY_EDITOR
        private bool m_HierarchyMode;
        private bool m_SavedHierarchyMode;
#endif
        public HierarchyModeAttribute(bool hierarchyMode)
        {
#if UNITY_EDITOR
            m_HierarchyMode = hierarchyMode;
#endif
        }
#if UNITY_EDITOR
        public override bool BeforeGetHeight(ref SerializedProperty property, ref GUIContent label, ref bool includeChildren)
        {
            m_SavedHierarchyMode = EditorGUIUtility.hierarchyMode;
            if (!IsVisible()) return true;
            EditorGUIUtility.hierarchyMode = m_HierarchyMode;
            return true;
        }

        public override void AfterGetHeight(SerializedProperty property, GUIContent label, bool includeChildren)
        {
            EditorGUIUtility.hierarchyMode = m_SavedHierarchyMode;
        }

        public override bool BeforeGUI(ref Rect position, ref SerializedProperty property, ref GUIContent label, ref bool includeChildren, bool visible)
        {
            m_SavedHierarchyMode = EditorGUIUtility.hierarchyMode;
            if (!IsVisible()) return visible;
            EditorGUIUtility.hierarchyMode = m_HierarchyMode;
            return visible;
        }

        public override void AfterGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUIUtility.hierarchyMode = m_SavedHierarchyMode;
        }
#endif
    }
}