using UnityEngine;
using System.Linq;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StackableDecorator
{
    public class EnumPopupAttribute : StackableFieldAttribute
    {
        public string names = null;
        public string exclude = string.Empty;
        public string placeHolder = string.Empty;
#if UNITY_EDITOR
        private List<int> m_Indexs = null;
        private string[] m_Exclude = null;
        private string[] m_Names = null;

        private static GUIContent m_Content = new GUIContent(" ");
        private static GUIStyle m_Style = null;

        private DynamicValue<string[]> m_DynamicNames = new DynamicValue<string[]>();
#endif
        public EnumPopupAttribute()
        {
#if UNITY_EDITOR
#endif
        }
#if UNITY_EDITOR
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label, bool includeChildren)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label, bool includeChildren)
        {
            if (property.propertyType != SerializedPropertyType.Enum)
            {
                EditorGUI.LabelField(position, label.text, "Use with Enum.");
                return;
            }

            if (m_Style == null)
            {
                m_Style = new GUIStyle(EditorStyles.popup);
                m_Style.normal.background = null;
            }

            if (m_Indexs == null)
            {
                m_Indexs = new List<int>();
                m_Exclude = exclude.Split(',');
                for (int i = 0; i < property.enumNames.Length; i++)
                    if (!m_Exclude.Contains(property.enumNames[i]))
                        m_Indexs.Add(i);
            }

            int selected = m_Indexs.IndexOf(property.enumValueIndex);
            m_DynamicNames.UpdateAndCheckInitial(this.names, property);

            if (m_Names == null && this.names != null)
                m_Names = this.names.Split(',');
            if (!m_DynamicNames.IsStatic())
                m_Names = m_DynamicNames.GetValue();

            var names = this.names == null ? property.enumDisplayNames.Except(m_Exclude).ToArray() : m_Names;

            label = EditorGUI.BeginProperty(position, label, property);
            selected = EditorGUI.Popup(position, label.text, selected, names);
            if (selected < 0 || selected >= names.Length)
            {
                var rect = EditorGUI.PrefixLabel(position, m_Content);
                GUI.Label(rect, placeHolder, m_Style);
            }
            else
                property.enumValueIndex = m_Indexs[selected];
            EditorGUI.EndProperty();

        }
#endif
    }
}