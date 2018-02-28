using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StackableDecorator
{
    public class ExpandableAttribute : StackableFieldAttribute
    {
#if UNITY_EDITOR
        private GUIContent m_Content = new GUIContent(" ");
#endif
        public ExpandableAttribute()
        {
#if UNITY_EDITOR
#endif
        }
#if UNITY_EDITOR
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label, bool includeChildren)
        {
            if (HasVisibleChildFields(property))
                return base.GetPropertyHeight(property, GUIContent.none, includeChildren);
            else
                return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label, bool includeChildren)
        {
            if (HasVisibleChildFields(property))
                base.OnGUI(position, property, GUIContent.none, includeChildren);
            else
            {
                label = EditorGUI.BeginProperty(position, label, property);
                var rect = EditorGUI.PrefixLabel(position, m_Content);
                position.xMax = rect.xMin;
                property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label, true);
                var indentLevel = EditorGUI.indentLevel;
                EditorGUI.indentLevel = 0;
                DefaultPropertyField(rect, property, GUIContent.none);
                EditorGUI.indentLevel = indentLevel;
                EditorGUI.EndProperty();
            }
        }
#endif
    }
}