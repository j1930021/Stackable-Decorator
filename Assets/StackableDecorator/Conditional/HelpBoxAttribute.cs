using UnityEngine;
using System.Reflection;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StackableDecorator
{
    public class HelpBoxAttribute : ConditionalAttribute
    {
        public bool indented = true;
        public MessageType messageType = MessageType.Info;
        public bool below = true;
        public float height = -1;
        public bool all = true;
#if UNITY_EDITOR
        private string m_Message;

        private DynamicValue<string> m_DynamicMessage = null;
        private Dictionary<string, Data> m_Data = new Dictionary<string, Data>();

        private class Data
        {
            public float height;
            public float boxHeight;
            public GUIContent content = null;
        }
#endif
        public HelpBoxAttribute(string message) : base(true)
        {
#if UNITY_EDITOR
            m_Message = message;
#endif
        }

        public HelpBoxAttribute(string message, string condition) : base(condition)
        {
#if UNITY_EDITOR
            m_Message = message;
#endif
        }
#if UNITY_EDITOR
        private string GetMessage()
        {
            if (m_DynamicMessage == null)
                m_DynamicMessage = new DynamicValue<string>(m_Message, m_SerializedProperty);
            m_DynamicMessage.Update(m_SerializedProperty);
            return m_DynamicMessage.GetValue();
        }

       public override float GetHeight(SerializedProperty property, GUIContent label, float height)
        {
            if (!IsVisible()) return height;
            if (all ? !MatchAll() : !MatchAny()) return height;

            var data = m_Data.Get(property.propertyPath);
            data.height = height;
            if (data.content == null)
            {
                var method = typeof(EditorGUIUtility).GetMethod("GetHelpIcon", BindingFlags.Static | BindingFlags.NonPublic);
                var icon = (Texture2D)method.Invoke(null, new object[] { (UnityEditor.MessageType)messageType });
                data.content = new GUIContent(icon);
            }
            data.content.text = GetMessage();
            data.boxHeight = EditorStyles.helpBox.CalcSize(data.content).y;
            if (this.height >= 0)
                data.boxHeight = this.height;

            return height + data.boxHeight + 2;
        }

        public override bool BeforeGUI(ref Rect position, ref SerializedProperty property, ref GUIContent label, ref bool includeChildren, bool visible)
        {
            if (!IsVisible()) return visible;
            if (!visible) return false;

            var data = m_Data.Get(property.propertyPath);
            if (all ? MatchAll() : MatchAny())
            {
                var indent = indented ? EditorGUI.IndentedRect(position) : new Rect(position);
                position.height = data.height;
                indent.height = data.boxHeight;
                if (!below)
                    position.y += data.boxHeight + 2;
                else
                    indent.y += data.height + 2;
                GUI.Label(indent, data.content, EditorStyles.helpBox);

                data.boxHeight = EditorStyles.helpBox.CalcHeight(data.content, indent.width);
            }

            return true;
        }
#endif
    }
}