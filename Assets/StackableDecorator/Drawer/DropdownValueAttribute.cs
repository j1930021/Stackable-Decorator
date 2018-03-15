using UnityEngine;
using System;
using System.Linq;
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
        private Type m_TargetType = null;

        private string[] m_Names = null;
        private object[] m_Values = null;

        private DynamicValue<string[]> m_DynamicNames = new DynamicValue<string[]>();
        private DynamicValue<object> m_DynamicValues = new DynamicValue<object>();

        private static GUIStyle s_Style = null;
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
#if UNITY_5_6_OR_NEWER
                case SerializedPropertyType.ExposedReference:
#endif
#if UNITY_2017_1_OR_NEWER
                case SerializedPropertyType.FixedBufferSize:
#endif
                    EditorGUI.LabelField(position, label.text, "Not supported.");
                    return;
            }

            if (s_Style == null)
            {
                s_Style = new GUIStyle(EditorStyles.popup);
                s_Style.normal.background = null;
            }

            if (m_TargetType == null)
            {
                m_TargetType = m_FieldInfo.FieldType;
                if (m_TargetType.IsArrayOrList())
                    m_TargetType = m_TargetType.GetArrayOrListElementType();
                m_TargetType = m_TargetType.MakeArrayType();
            }

            m_DynamicNames.UpdateAndCheckInitial(names, property);
            m_DynamicValues.UpdateAndCheckInitial(m_ValuesGetter, property, m_TargetType);

            if (m_Names == null)
            {
                var values = (Array)m_DynamicValues.GetValue();
                if (values == null)
                {
                    EditorGUI.LabelField(position, label.text, "Getter not correct.");
                    return;
                }

                m_Values = values.Cast<object>().ToArray();
                if (names == null)
                    m_Names = m_Values.Select(v => v.ToString()).ToArray();
                else
                {
                    var array = m_DynamicNames.GetValue();
                    m_Names = array == null ? names.Split(',') : array;
                }

                var length = Mathf.Min(m_Names.Length, m_Values.Length);
                m_Names = m_Names.Take(length).ToArray();
                m_Values = m_Values.Take(length).ToArray();
            }

            if (m_Values == null)
            {
                EditorGUI.LabelField(position, label.text, "Getter not correct.");
                return;
            }

            var propertyValue = property.GetValueAsObject();
            int selected = Array.IndexOf(m_Values, propertyValue);

            label = EditorGUI.BeginProperty(position, label, property);
            var value = EditorGUI.Popup(position, label.text, selected, m_Names);
            if (value < 0 || value >= m_Names.Length)
            {
                if (!property.hasMultipleDifferentValues)
                    EditorGUI.LabelField(position, " ", placeHolder, s_Style);
            }
            else if (value != selected)
                property.SetObjectToValue(m_Values[value]);
            EditorGUI.EndProperty();
        }
#endif
            }
        }