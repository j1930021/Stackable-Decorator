using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StackableDecorator
{
    public class IncludeChildrenAttribute : StackableDecoratorAttribute
    {
#if UNITY_EDITOR
        private bool m_IncludeChildren;
#endif
        public IncludeChildrenAttribute(bool includeChildren)
        {
#if UNITY_EDITOR
            m_IncludeChildren = includeChildren;
#endif
        }
#if UNITY_EDITOR
        public override bool BeforeGetHeight(ref SerializedProperty property, ref GUIContent label, ref bool includeChildren)
        {
            if (!IsVisible()) return true;
            includeChildren = m_IncludeChildren;
            return true;
        }

        public override bool BeforeGUI(ref Rect position, ref SerializedProperty property, ref GUIContent label, ref bool includeChildren, bool visible)
        {
            if (!IsVisible()) return visible;
            includeChildren = m_IncludeChildren;
            return visible;
        }
#endif
    }
}