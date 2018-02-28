using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StackableDecorator
{
    public class OnValueChangedAttribute : StackableDecoratorAttribute
    {
#if UNITY_EDITOR
        private bool check = false;
        private string m_Action;
        private DynamicAction m_DynamicAction = null;
#endif
        public OnValueChangedAttribute(string action)
        {
#if UNITY_EDITOR
            m_Action = action;
#endif
        }
#if UNITY_EDITOR
        private void Update()
        {
            if (m_DynamicAction == null)
                m_DynamicAction = new DynamicAction(m_Action, m_SerializedProperty);
            m_DynamicAction.Update(m_SerializedProperty);
        }

        public override bool BeforeGUI(ref Rect position, ref SerializedProperty property, ref GUIContent label, ref bool includeChildren, bool visible)
        {
            if (!IsVisible()) return visible;
            if (!visible) return false;
            check = true;
            EditorGUI.BeginChangeCheck();
            return true;
        }

        public override void AfterGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!IsVisible()) return;
            if (!check) return;
            check = false;
            Update();
            if (EditorGUI.EndChangeCheck())
                m_DynamicAction.DoAction();
        }
#endif
    }
}