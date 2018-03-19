using UnityEngine;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

namespace StackableDecorator
{
    public class TagPopupAttribute : StackableFieldAttribute
    {
        public string exclude = string.Empty;
        public string placeHolder = string.Empty;
#if UNITY_EDITOR
        private string[] m_Exclude = null;

        private static GUIStyle s_Style = null;
#endif
        public TagPopupAttribute()
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
            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.LabelField(position, label.text, "Use with String.");
                return;
            }

            if (s_Style == null)
            {
                s_Style = new GUIStyle(EditorStyles.popup);
                s_Style.normal.background = null;
            }

            if (m_Exclude == null)
                m_Exclude = exclude.Split(',');

            var tags = InternalEditorUtility.tags.Except(m_Exclude).ToArray();

            int selected = ArrayUtility.IndexOf(tags, property.stringValue);
            label = EditorGUI.BeginProperty(position, label, property);
            var value = EditorGUI.Popup(position, label.text, selected, tags);
            if (value < 0 || value >= tags.Length)
            {
                if (!property.hasMultipleDifferentValues)
                    EditorGUI.LabelField(position, " ", placeHolder, s_Style);
            }
            else if (value != selected)
                property.stringValue = tags[value];
            EditorGUI.EndProperty();
        }
#endif
    }
}