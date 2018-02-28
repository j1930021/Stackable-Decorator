using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StackableDecorator
{
    public class InGroupAttribute : StackableDecoratorAttribute
    {
#if UNITY_EDITOR
        private string[] m_Names;
        private static HashSet<string> s_GroupSet = new HashSet<string>();
        private static Stack<HashSet<string>> s_GroupStack = new Stack<HashSet<string>>();
#endif
        public InGroupAttribute(string name)
        {
#if UNITY_EDITOR
            m_Names = name.Split(',');
#endif
        }
#if UNITY_EDITOR
        public static void Add(string name)
        {
            s_GroupSet.Add(name);
        }

        public static void Remove(string name)
        {
            s_GroupSet.Remove(name);
        }

        public static void Push()
        {
            s_GroupStack.Push(s_GroupSet);
            s_GroupSet = new HashSet<string>();
        }

        public static void Pop()
        {
            s_GroupSet = s_GroupStack.Pop();
        }

        public override bool BeforeGetHeight(ref SerializedProperty property, ref GUIContent label, ref bool includeChildren)
        {
            if (!IsVisible()) return true;
            return s_GroupSet.Overlaps(m_Names);
        }

        public override float GetHeight(SerializedProperty property, GUIContent label, float height)
        {
            if (!IsVisible()) return height;
            return s_GroupSet.Overlaps(m_Names) ? height : 0;
        }

        public override bool BeforeGUI(ref Rect position, ref SerializedProperty property, ref GUIContent label, ref bool includeChildren, bool visible)
        {
            if (!IsVisible()) return visible;
            if (!visible) return false;
            return s_GroupSet.Overlaps(m_Names);
        }
#endif
    }
}