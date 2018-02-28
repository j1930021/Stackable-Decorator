using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StackableDecorator
{
    public class AsStringAttribute : StackableFieldAttribute
    {
        public bool label = true;
        public bool icon = true;
        public bool tooltip = true;
#if UNITY_EDITOR
        private static GUIStyle s_Style = null;
        private static GUIContent s_Content = new GUIContent();
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
            s_Content.text = property.AsString();
            if (this.label)
            {
                label = EditorGUI.BeginProperty(position, label, property);
                position = EditorGUI.PrefixLabel(position, label);
            }
            else
            {
                if (icon) s_Content.image = label.image;
                if (tooltip) s_Content.tooltip = label.tooltip;
            }
            GUI.Label(position, s_Content, s_Style);
            if (this.label)
                EditorGUI.EndProperty();
        }
#endif
    }
}