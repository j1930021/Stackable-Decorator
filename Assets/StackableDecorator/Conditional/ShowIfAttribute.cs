using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StackableDecorator
{
    public class ShowIfAttribute : ConditionalAttribute
    {
        public bool enable = true;
        public bool disable = true;
        public bool all = true;
#if UNITY_EDITOR
#endif
        public ShowIfAttribute(bool condition) : base(condition)
        {
#if UNITY_EDITOR
#endif
        }

        public ShowIfAttribute(string condition) : base(condition)
        {
#if UNITY_EDITOR
#endif
        }
#if UNITY_EDITOR
        public override bool BeforeGetHeight(ref SerializedProperty property, ref GUIContent label, ref bool includeChildren)
        {
            if (!IsVisible()) return true;
            var condition = all ? MatchAll() : MatchAny();
            if (!condition && disable)
                return false;
            return true;
        }

        public override float GetHeight(SerializedProperty property, GUIContent label, float height)
        {
            if (!IsVisible()) return height;
            var condition = all ? MatchAll() : MatchAny();
            if (!condition && disable)
                return 0;
            return height;
        }

        public override bool BeforeGUI(ref Rect position, ref SerializedProperty property, ref GUIContent label, ref bool includeChildren, bool visible)
        {
            if (!IsVisible()) return visible;
            if (!visible) return false;
            var condition = all ? MatchAll() : MatchAny();
            if (!condition && disable)
                visible = false;
            return visible;
        }
#endif
    }
}