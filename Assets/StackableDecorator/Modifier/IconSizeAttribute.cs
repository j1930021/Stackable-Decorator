using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StackableDecorator
{
    public class IconSizeAttribute : StackableDecoratorAttribute
    {
#if UNITY_EDITOR
        private float m_Width;
        private float m_Height;
        private Vector2 m_SavedSize;
#endif
        public IconSizeAttribute(int width, int height)
        {
#if UNITY_EDITOR
            m_Width = width;
            m_Height = height;
#endif
        }
#if UNITY_EDITOR
        public override bool BeforeGetHeight(ref SerializedProperty property, ref GUIContent label, ref bool includeChildren)
        {
            m_SavedSize = EditorGUIUtility.GetIconSize();
            if (!IsVisible()) return true;
            EditorGUIUtility.SetIconSize(new Vector2(m_Width, m_Height));
            return true;
        }

        public override void AfterGetHeight(SerializedProperty property, GUIContent label, bool includeChildren)
        {
            EditorGUIUtility.SetIconSize(m_SavedSize);
        }

        public override bool BeforeGUI(ref Rect position, ref SerializedProperty property, ref GUIContent label, ref bool includeChildren, bool visible)
        {
            m_SavedSize = EditorGUIUtility.GetIconSize();
            if (!IsVisible()) return visible;
            EditorGUIUtility.SetIconSize(new Vector2(m_Width, m_Height));
            return visible;
        }

        public override void AfterGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUIUtility.SetIconSize(m_SavedSize);
        }
#endif
    }
}