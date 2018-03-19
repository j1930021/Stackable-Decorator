using UnityEngine;
using UnityEditorInternal;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StackableDecorator
{
    public class LayerPopupAttribute : StackableFieldAttribute
    {
        public string exclude = string.Empty;
        public string placeHolder = string.Empty;
#if UNITY_EDITOR
        private string[] m_Exclude = null;

        private static GUIStyle s_Style = null;
#endif
        public LayerPopupAttribute()
        {
#if UNITY_EDITOR
#endif
        }
#if UNITY_EDITOR
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label, bool includeChildren)
        {
            if (property.propertyType != SerializedPropertyType.Integer)
            {
                EditorGUI.LabelField(position, label.text, "Use with int.");
                return;
            }

            if (s_Style == null)
            {
                s_Style = new GUIStyle(EditorStyles.popup);
                s_Style.normal.background = null;
            }

            if (m_Exclude == null)
                m_Exclude = exclude.Split(',');

            var layers = InternalEditorUtility.layers.Except(m_Exclude).ToArray();
            var layer = LayerMask.LayerToName(property.intValue);

            int selected = ArrayUtility.IndexOf(layers, layer);
            label = EditorGUI.BeginProperty(position, label, property);
            var value = EditorGUI.Popup(position, label.text, selected, layers);
            if (value < 0 || value >= layers.Length)
            {
                if (!property.hasMultipleDifferentValues)
                    EditorGUI.LabelField(position, " ", placeHolder, s_Style);
            }
            else if (value != selected)
                property.intValue = LayerMask.NameToLayer(layers[value]);
            EditorGUI.EndProperty();
        }
#endif
    }
}