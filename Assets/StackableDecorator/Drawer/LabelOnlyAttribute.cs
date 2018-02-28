using UnityEngine;
using UnityEngine.Profiling;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StackableDecorator
{
    public class LabelOnlyAttribute : StackableFieldAttribute
    {
#if UNITY_EDITOR
        private static GUIStyle s_Style = null;
#endif
        public LabelOnlyAttribute()
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
            }
            GUI.Label(position, label, s_Style);
        }
#endif
    }
}