using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StackableDecorator
{
    public class AsStringAttribute : StackableFieldAttribute
    {
        public bool label = false;
        public bool icon = false;
        public bool tooltip = false;
#if UNITY_EDITOR
        private GUIContent m_Content = new GUIContent();

        private static GUIStyle s_Style = null;
#endif
        public AsStringAttribute()
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
            if (s_Style == null)
            {
                s_Style = new GUIStyle("label");
                s_Style.padding = new RectOffset();
                s_Style.alignment = TextAnchor.MiddleLeft;
                s_Style.clipping = TextClipping.Clip;
            }
            m_Content.text = property.AsString();
            if (icon) m_Content.image = label.image;
            if (tooltip) m_Content.tooltip = label.tooltip;
            if (this.label)
                EditorGUI.LabelField(position, label, m_Content, s_Style);
            else
                EditorGUI.LabelField(position, m_Content, s_Style);
        }
#endif
    }
}