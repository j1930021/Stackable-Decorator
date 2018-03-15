using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System.Text;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

namespace StackableDecorator
{
    public class LayerMaskPopupAttribute : StackableFieldAttribute
    {
        public string placeHolder = string.Empty;
        public bool showAll = true;
#if UNITY_EDITOR && UNITY_5_6_OR_NEWER
        private List<string> m_Names = null;
        private List<long> m_Values = null;

        private static GUIContent s_Content = new GUIContent();
        private static StringBuilder s_StringBuilder = new StringBuilder();
        private static int s_HashCode = "StackableDecorator.LayerMaskPopupAttribute".GetHashCode();
#endif
        public LayerMaskPopupAttribute()
        {
#if UNITY_EDITOR && UNITY_5_6_OR_NEWER
#endif
        }
#if UNITY_EDITOR
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label, bool includeChildren)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label, bool includeChildren)
        {
#if !UNITY_5_6_OR_NEWER
            EditorGUI.LabelField(position, label.text, "Use with Unity 5.6 or above.");
            return;
#else
            if (property.propertyType != SerializedPropertyType.LayerMask)
            {
                EditorGUI.LabelField(position, label.text, "Use with LayerMask.");
                return;
            }

            if (m_Names == null)
            {
                m_Names = new List<string>(InternalEditorUtility.layers);
                m_Values = m_Names.Select(n => (long)LayerMask.GetMask(n)).ToList();
            }

            long selected = property.intValue;
            long allmask = 0;
            foreach (var mask in m_Values)
                allmask |= mask;
            var id = GUIUtility.GetControlID(s_HashCode, FocusType.Passive, position);

            label = EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, label);

            s_Content.tooltip = string.Empty;
            if (selected == 0)
                s_Content.text = placeHolder == string.Empty ? "None" : placeHolder;
            else if (showAll && selected == allmask)
                s_Content.text = "All";
            else
            {
                s_StringBuilder.Length = 0;
                for (int i = 0; i < m_Names.Count; i++)
                    if ((selected & m_Values[i]) == m_Values[i])
                        s_StringBuilder.Append(m_Names[i]).Append(", ");
                if (s_StringBuilder.Length > 0)
                    s_StringBuilder.Length -= 2;
                s_Content.text = s_StringBuilder.ToString();
                if (EditorStyles.popup.CalcSize(s_Content).x > position.width)
                    s_Content.tooltip = s_Content.text;
            }

            if (property.hasMultipleDifferentValues)
                selected = 0;
            if (EditorGUI.DropdownButton(position, s_Content, FocusType.Keyboard, EditorStyles.popup))
                MaskPopupList.Popup(position, selected, m_Names, m_Values, false, id);
            if (MaskPopupList.IsSelectionChanged(id))
                property.intValue = (int)MaskPopupList.GetLastSelectedValue();
            EditorGUI.EndProperty();
#endif
        }
#endif
    }
}