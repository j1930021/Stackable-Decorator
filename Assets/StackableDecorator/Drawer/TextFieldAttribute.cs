using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StackableDecorator
{
    public class TextFieldAttribute : StackableFieldAttribute
    {
        private const int kLineHeight = 13;

        public string placeHolder = string.Empty;
#if UNITY_EDITOR
        private int m_Lines = 1;
#endif
        public TextFieldAttribute()
        {
#if UNITY_EDITOR
#endif
        }

        public TextFieldAttribute(int lines)
        {
#if UNITY_EDITOR
            m_Lines = lines;
#endif
        }
#if UNITY_EDITOR
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label, bool includeChildren)
        {
            if (m_Lines < 1) m_Lines = 1;
            var result = EditorGUIUtility.singleLineHeight;
            if (m_Lines > 1 && !EditorGUIUtility.wideMode)
                result += EditorGUIUtility.singleLineHeight;
            result += (m_Lines - 1) * kLineHeight;
            return result;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label, bool includeChildren)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.LabelField(position, label.text, "Use with String.");
                return;
            }

            if (m_Lines < 1) m_Lines = 1;
            int indentLevel = EditorGUI.indentLevel;
            string stringValue = property.stringValue;

            label = EditorGUI.BeginProperty(position, label, property);
            var rect = EditorGUI.PrefixLabel(position, label);
            EditorGUI.indentLevel = 0;
            if (m_Lines == 1)
                stringValue = EditorGUI.TextField(rect, GUIContent.none, stringValue);
            else
            {
                if (EditorGUIUtility.wideMode)
                {
                    stringValue = EditorGUI.TextArea(rect, stringValue);
                }
                else
                {
                    EditorGUI.indentLevel = EditorGUI.indentLevel + 1;
                    rect = position.CutTop(16).indent(true);
                    EditorGUI.indentLevel = 0;
                    stringValue = EditorGUI.TextArea(rect, stringValue);
                }
            }
            if (stringValue != property.stringValue)
                property.stringValue = stringValue;
            EditorGUI.EndProperty();

            EditorGUI.indentLevel = indentLevel;

            if (!property.hasMultipleDifferentValues)
                if (string.IsNullOrEmpty(property.stringValue))
                    if (!string.IsNullOrEmpty(placeHolder))
                        using (new EditorGUI.DisabledScope(true))
                            GUI.Label(rect.Shrink(2, 0), placeHolder, EditorStyles.wordWrappedLabel);
        }
#endif
    }
}