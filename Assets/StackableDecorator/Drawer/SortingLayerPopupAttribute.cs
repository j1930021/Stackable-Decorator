using UnityEngine;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StackableDecorator
{
    public class SortingLayerPopupAttribute : StackableFieldAttribute
    {
        public string exclude = string.Empty;
        public string placeHolder = string.Empty;
#if UNITY_EDITOR
        private string[] m_Exclude = null;

        private static GUIStyle s_Style = null;
#endif
        public SortingLayerPopupAttribute()
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
            if (property.propertyType != SerializedPropertyType.String && property.propertyType != SerializedPropertyType.Integer)
            {
                EditorGUI.LabelField(position, label.text, "Use with String or int.");
                return;
            }

            if (s_Style == null)
            {
                s_Style = new GUIStyle(EditorStyles.popup);
                s_Style.normal.background = null;
            }

            if (m_Exclude == null)
                m_Exclude = exclude.Split(',');

            var layers = SortingLayer.layers.Select(l => l.name).Except(m_Exclude).ToArray();
            string layer = string.Empty;
            if (property.propertyType == SerializedPropertyType.String)
                layer = property.stringValue;
            if (property.propertyType == SerializedPropertyType.Integer)
                layer = SortingLayer.IDToName(property.intValue);

            int selected = ArrayUtility.IndexOf(layers, layer);
            label = EditorGUI.BeginProperty(position, label, property);
            var value = EditorGUI.Popup(position, label.text, selected, layers);
            if (value < 0 || value >= layers.Length)
            {
                if (!property.hasMultipleDifferentValues)
                    EditorGUI.LabelField(position, " ", placeHolder, s_Style);
            }
            else if (value != selected)
            {
                if (property.propertyType == SerializedPropertyType.Integer)
                    property.intValue = SortingLayer.NameToID(layers[value]);
                else
                    property.stringValue = layers[value];
            }
            EditorGUI.EndProperty();
        }
#endif
    }
}