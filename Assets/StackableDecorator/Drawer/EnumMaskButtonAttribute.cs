using UnityEngine;
using System.Linq;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StackableDecorator
{
    public class EnumMaskButtonAttribute : StackableFieldAttribute
    {
        public string styles = null;
        public bool all = true;
        public string exclude = string.Empty;
        public int column = -1;
        public int hOffset = 0;
        public int vOffset = 0;
#if UNITY_EDITOR
        private ButtonMask m_ButtonMask = null;
#endif
        public EnumMaskButtonAttribute()
        {
#if UNITY_EDITOR
#endif
        }
#if UNITY_EDITOR
        private ButtonMask GetButtonMask()
        {
            if (m_ButtonMask == null)
            {
                var names = Enum.GetNames(m_FieldInfo.FieldType).Except(exclude.Split(',')).ToArray();
                //var values = Array.ConvertAll(Enum.GetValues(m_FieldInfo.FieldType).Cast<Enum>().ToArray(), value => Convert.ToInt64(value));
                var values = names.Select(n => Convert.ToInt64(Enum.Parse(m_FieldInfo.FieldType, n))).ToArray();
                m_ButtonMask = new ButtonMask(names, values, all, styles == null ? EditorStyles.miniButton.name : styles);
            }
            m_ButtonMask.hOffset = hOffset;
            m_ButtonMask.vOffset = vOffset;
            return m_ButtonMask;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label, bool includeChildren)
        {
            var height = base.GetPropertyHeight(property, label, includeChildren);
            if (property.propertyType != SerializedPropertyType.Enum) return height;
            return Mathf.Max(height, GetButtonMask().GetButtonSize(column).y);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label, bool includeChildren)
        {
            if (property.propertyType != SerializedPropertyType.Enum)
            {
                EditorGUI.LabelField(position, label.text, "Use with Enum.");
                return;
            }

            if (column == -1) column = GetButtonMask().GetCount();

            label = EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, label);
            position.height = GetButtonMask().GetButtonSize(column).y;
            int longValue = (int)GetButtonMask().Draw(position, property.longValue, column);
            if (longValue != property.longValue)
                property.longValue = longValue;
            EditorGUI.EndProperty();
        }
#endif
    }
}