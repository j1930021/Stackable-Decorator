using UnityEngine;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StackableDecorator
{
    public class EnumButtonAttribute : StackableFieldAttribute
    {
        public string styles = null;
        public string exclude = string.Empty;
        public int column = -1;
        public int hOffset = 0;
        public int vOffset = 0;
#if UNITY_EDITOR
        private ButtonGroup m_ButtonGroup = null;
#endif
        public EnumButtonAttribute()
        {
#if UNITY_EDITOR
#endif
        }
#if UNITY_EDITOR
        private ButtonGroup GetButtonGroup()
        {
            if (m_ButtonGroup == null)
                m_ButtonGroup = new ButtonGroup(m_SerializedProperty.enumDisplayNames.Except(exclude.Split(',')).ToArray(), styles == null ? EditorStyles.miniButton.name : styles);
            m_ButtonGroup.hOffset = hOffset;
            m_ButtonGroup.vOffset = vOffset;
            return m_ButtonGroup;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label, bool includeChildren)
        {
            var height = base.GetPropertyHeight(property, label, includeChildren);
            if (property.propertyType != SerializedPropertyType.Enum) return height;
            return Mathf.Max(height, GetButtonGroup().GetButtonSize(column).y);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label, bool includeChildren)
        {
            if (property.propertyType != SerializedPropertyType.Enum)
            {
                EditorGUI.LabelField(position, label.text, "Use with Enum.");
                return;
            }

            if (column == -1) column = GetButtonGroup().GetCount();

            label = EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, label);
            position.height = GetButtonGroup().GetButtonSize(column).y;
            int enumValueIndex = (int)GetButtonGroup().Draw(position, property.enumValueIndex, column);
            if (enumValueIndex != property.enumValueIndex)
                property.enumValueIndex = enumValueIndex;
            EditorGUI.EndProperty();
        }
#endif
    }
}