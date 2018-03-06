using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StackableDecorator
{
    public class EnableIfAttribute : ConditionalAttribute
    {
        public bool enable = true;
        public bool disable = true;
        public bool all = true;
#if UNITY_EDITOR
        private bool m_SavedEnabled;
#endif
        public EnableIfAttribute(bool condition) : base(condition)
        {
#if UNITY_EDITOR
#endif
        }

        public EnableIfAttribute(string condition) : base(condition)
        {
#if UNITY_EDITOR
#endif
        }
#if UNITY_EDITOR
        public override float GetHeight(SerializedProperty property, GUIContent label, float height)
        {
            return height;
        }

        public override bool BeforeGUI(ref Rect position, ref SerializedProperty property, ref GUIContent label, ref bool includeChildren, bool visible)
        {
            if (!IsVisible()) return visible;
            if (!visible) return false;
            m_SavedEnabled = GUI.enabled;
            var condition = all ? MatchAll() : MatchAny();
            if (condition && enable)
                GUI.enabled = true;
            if (!condition && disable)
                GUI.enabled = false;
            return visible;
        }

        public override void AfterGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.enabled = m_SavedEnabled;
        }
#endif
    }
}