using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StackableDecorator
{
    public class ToggleLeftAttribute : StackableFieldAttribute
    {
#if UNITY_EDITOR
#endif
        public ToggleLeftAttribute()
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
            if (property.propertyType != SerializedPropertyType.Boolean)
            {
                EditorGUI.LabelField(position, label.text, "Use with bool.");
                return;
            }
            label = EditorGUI.BeginProperty(position, label, property);
            var value = EditorGUI.ToggleLeft(position, label, property.boolValue);
            if (value != property.boolValue)
                property.boolValue = value;
            EditorGUI.EndProperty();
        }
#endif
    }
}