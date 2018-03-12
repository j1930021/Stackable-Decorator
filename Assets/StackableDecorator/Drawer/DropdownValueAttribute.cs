using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StackableDecorator
{
    public class DropdownValueAttribute : StackableFieldAttribute
    {
        public string names = null;
        public string placeHolder = string.Empty;
#if UNITY_EDITOR
        private string m_ValuesGetter;
        private string[] m_NameArray = null;

        private string[] m_Names = new string[0];
        private object[] m_Values = new object[0];

        private static GUIContent m_Content = new GUIContent(" ");
        private static GUIStyle m_Style = null;

        private DynamicValue<string[]> m_DynamicNames = new DynamicValue<string[]>();
        private DynamicValue<object> m_DynamicValues = new DynamicValue<object>();
#endif
        public DropdownValueAttribute(string values)
        {
#if UNITY_EDITOR
            m_ValuesGetter = values;
#endif
        }
#if UNITY_EDITOR
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label, bool includeChildren)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label, bool includeChildren)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.Generic:
                case SerializedPropertyType.Enum:
                case SerializedPropertyType.LayerMask:
                case SerializedPropertyType.ArraySize:
                case SerializedPropertyType.Gradient:
                case SerializedPropertyType.ExposedReference:
                case SerializedPropertyType.FixedBufferSize:
                    EditorGUI.LabelField(position, label.text, "Not supported.");
                    return;
            }

            if (m_Style == null)
            {
                m_Style = new GUIStyle(EditorStyles.popup);
                m_Style.normal.background = null;
            }

            int selected = -1;
            var propertyValue = property.GetValueAsObject();
            m_DynamicNames.UpdateAndCheckInitial(names, property);
            m_DynamicValues.UpdateAndCheckInitial(m_ValuesGetter, property, m_FieldInfo.FieldType.MakeArrayType());
            var values = (Array)m_DynamicValues.GetValue();
            if (values == null)
            {
                EditorGUI.LabelField(position, label.text, "Getter not correct.");
                return;
            }

            if (m_NameArray == null && names != null)
                m_NameArray = names.Split(',');
            if (names != null && !m_DynamicNames.IsStatic())
                m_NameArray = m_DynamicNames.GetValue();

            if (names != null && m_NameArray != null)
            {
                var length = Math.Min(names.Length, values.Length);
                if (m_Names.Length != length)
                    m_Names = new string[length];
                if (m_Values.Length != length)
                    m_Values = new object[length];
                for (int i = 0; i < length; i++)
                {
                    m_Names[i] = m_NameArray[i];
                    m_Values[i] = values.GetValue(i);
                    if (propertyValue.Equals(m_Values[i]))
                        selected = i;
                }
            }
            else
            {
                var length = values.Length;
                if (m_Names.Length != length)
                    m_Names = new string[length];
                if (m_Values.Length != length)
                    m_Values = new object[length];
                for (int i = 0; i < length; i++)
                {
                    m_Names[i] = values.GetValue(i).ToString();
                    m_Values[i] = values.GetValue(i);
                    if (propertyValue.Equals(m_Values[i]))
                        selected = i;
                }
            }

            label = EditorGUI.BeginProperty(position, label, property);
            selected = EditorGUI.Popup(position, label.text, selected, m_Names);
            if (selected < 0 || selected >= names.Length)
            {
                var rect = EditorGUI.PrefixLabel(position, m_Content);
                GUI.Label(rect, placeHolder, m_Style);
            }
            else
                property.SetObjectToValue(m_Values[selected]);
            EditorGUI.EndProperty();
        }
#endif
    }
}