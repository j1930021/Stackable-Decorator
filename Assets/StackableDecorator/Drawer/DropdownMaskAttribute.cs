using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System.Text;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StackableDecorator
{
    public class DropdownMaskAttribute : StackableFieldAttribute
    {
        public string placeHolder = string.Empty;
        public bool showAll = true;
        public bool showCombined = false;
        public bool sortCombined = true;
#if UNITY_5_6_OR_NEWER && UNITY_EDITOR
        private string m_NamesGetter;
        private string m_ValuesGetter;
        private List<string> m_Names = null;
        private List<long> m_Values = null;

        private DynamicValue<string[]> m_DynamicNames = new DynamicValue<string[]>();
        private DynamicValue<long[]> m_DynamicValues = new DynamicValue<long[]>();
        private DynamicValue<int[]> m_DynamicIntValues = new DynamicValue<int[]>();

        private static GUIContent s_Content = new GUIContent();
        private static StringBuilder s_StringBuilder = new StringBuilder();
        private static int s_HashCode = "StackableDecorator.DropdownMaskAttribute".GetHashCode();
#endif
        public DropdownMaskAttribute(string names, string values)
        {
#if UNITY_5_6_OR_NEWER && UNITY_EDITOR
            m_NamesGetter = names;
            m_ValuesGetter = values;
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
            if (property.propertyType != SerializedPropertyType.Integer)
            {
                EditorGUI.LabelField(position, label.text, "Use with int or long.");
                return;
            }

            m_DynamicNames.UpdateAndCheckInitial(m_NamesGetter, property);
            m_DynamicValues.UpdateAndCheckInitial(m_ValuesGetter, property);
            m_DynamicIntValues.UpdateAndCheckInitial(m_ValuesGetter, property);

            if (m_Names == null)
            {
                var names = m_DynamicNames.GetValue();
                m_Names = names == null ? m_NamesGetter.Split(',').ToList() : names.ToList();
                var values = m_DynamicValues.GetValue();
                var values2 = m_DynamicIntValues.GetValue();
                m_Values = values != null ? values.ToList() : values2 != null ? values2.Select(v => (long)v).ToList() : null;

                if (m_Values == null)
                {
                    EditorGUI.LabelField(position, label.text, "Getter not correct.");
                    return;
                }

                int index;
                while ((index = m_Values.IndexOf(0)) >= 0)
                {
                    m_Names.RemoveAt(index);
                    m_Values.RemoveAt(index);
                }

                var length = Mathf.Min(m_Names.Count, m_Values.Count);
                m_Names = m_Names.Take(length).ToList();
                m_Values = m_Values.Take(length).ToList();
            }

            if (m_Values == null)
            {
                EditorGUI.LabelField(position, label.text, "Getter not correct.");
                return;
            }

            long selected = property.longValue;
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
                        if ((!sortCombined && showCombined) || m_Values[i].IsPowerOfTwo())
                            s_StringBuilder.Append(m_Names[i]).Append(", ");
                if (sortCombined)
                    for (int i = 0; i < m_Names.Count; i++)
                        if ((selected & m_Values[i]) == m_Values[i])
                            if (showCombined && !m_Values[i].IsPowerOfTwo())
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
                MaskPopupList.Popup(position, selected, m_Names, m_Values, sortCombined, id);
            if (MaskPopupList.IsSelectionChanged(id))
                property.longValue = MaskPopupList.GetLastSelectedValue();
            EditorGUI.EndProperty();
#endif
        }
#endif
    }
}