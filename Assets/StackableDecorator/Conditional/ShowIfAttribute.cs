using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StackableDecorator
{
    public class ShowIfAttribute : ConditionalAttribute
    {
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
            return all ? MatchAll() : MatchAny();
        }

        public override float GetHeight(SerializedProperty property, GUIContent label, float height)
        {
            if (!IsVisible()) return height;
            return (all ? MatchAll() : MatchAny()) ? height : 0;
        }

        public override bool BeforeGUI(ref Rect position, ref SerializedProperty property, ref GUIContent label, ref bool includeChildren, bool visible)
        {
            if (!IsVisible()) return visible;
            return all ? MatchAll() : MatchAny();
        }
#endif
    }
}