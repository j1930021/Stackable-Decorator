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
        private string[] m_Names = null;
        private List<int> m_Indexs = null;

        private DynamicValue<string[]> m_DynamicNames = new DynamicValue<string[]>();

        private static GUIStyle s_Style = null;
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

            if (s_Style == null)
            {
                s_Style = new GUIStyle(EditorStyles.popup);
                s_Style.normal.background = null;
            }

            m_DynamicNames.UpdateAndCheckInitial(names, property);

            if (m_Names == null)
            {
                var exclude = this.exclude.Split(',');
                m_Indexs = new List<int>();
                for (int i = 0; i < property.enumNames.Length; i++)
                    if (!exclude.Contains(property.enumNames[i]))
                        m_Indexs.Add(i);

                if (names == null)
                    m_Names = property.enumDisplayNames.Except(exclude).ToArray();
                else
                {
                    var array = m_DynamicNames.GetValue();
                    m_Names = array == null ? names.Split(',') : array;
                }
            }

            int selected = m_Indexs.IndexOf(property.enumValueIndex);
            label = EditorGUI.BeginProperty(position, label, property);
            var value = EditorGUI.Popup(position, label.text, selected, m_Names);
            if (value < 0 || value >= m_Names.Length)
            {
                if (!property.hasMultipleDifferentValues)
                    EditorGUI.LabelField(position, " ", placeHolder, s_Style);
            }
            else if (value != selected)
                property.enumValueIndex = m_Indexs[value];
            EditorGUI.EndProperty();
        }
#endif
    }
}