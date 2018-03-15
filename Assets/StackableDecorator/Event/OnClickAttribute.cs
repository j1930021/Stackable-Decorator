using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StackableDecorator
{
    public class OnClickAttribute : StackableDecoratorAttribute
    {
        public int button = 0;
        public bool use = true;
        public bool after = false;
#if UNITY_EDITOR
        private string m_Action;
        private DynamicAction m_DynamicAction = null;

        private static int s_HashCode = "StackableDecorator.OnClickAttribute".GetHashCode();
#endif
        public OnClickAttribute(string action)
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

        private void HandleEvent(Rect position)
        {
            var id = GUIUtility.GetControlID(s_HashCode, FocusType.Passive, position);
            var evt = Event.current;
            switch (evt.GetTypeForControl(id))
            {
                case EventType.MouseDown:
                    if (position.Contains(evt.mousePosition) && evt.button == button)
                    {
                        GUIUtility.hotControl = id;
                        if (use) evt.Use();
                    }
                    break;
                case EventType.MouseUp:
                    if (GUIUtility.hotControl == id && position.Contains(evt.mousePosition) && evt.button == button)
                    {
                        GUIUtility.hotControl = 0;
                        m_DynamicAction.DoAction();
                        if (use) evt.Use();
                    }
                    break;
            }
        }

        public override float GetHeight(SerializedProperty property, GUIContent label, float height)
        {
            return height;
        }

        public override bool BeforeGUI(ref Rect position, ref SerializedProperty property, ref GUIContent label, ref bool includeChildren, bool visible)
        {
            if (!IsVisible()) return visible;
            if (!visible) return false;

            if (!after)
            {
                Update();
                HandleEvent(position);
            }

            return true;
        }

        public override void AfterGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!IsVisible()) return;

            if (after)
            {
                Update();
                HandleEvent(position);
            }
        }
#endif
    }
}