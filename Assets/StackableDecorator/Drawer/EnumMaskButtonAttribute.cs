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
                var type = m_FieldInfo.FieldType;
                type = type.IsArrayOrList() ? type.GetArrayOrListElementType() : type;

                var names = Enum.GetNames(type).Except(exclude.Split(',')).ToList();
                var values = names.Select(n => Convert.ToInt64(Enum.Parse(type, n))).ToList();
                int index;
                while ((index = values.IndexOf(0)) >= 0)
                {
                    names.RemoveAt(index);
                    values.RemoveAt(index);
                }
                m_ButtonMask = new ButtonMask(names.ToArray(), values.ToArray(), all, styles == null ? EditorStyles.miniButton.name : styles);
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