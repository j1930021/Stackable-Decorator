using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System;
using System.Text;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StackableDecorator
{
    public class EnumMaskPopupAttribute : StackableFieldAttribute
    {
        public string names = null;
        public string exclude = string.Empty;
        public string placeHolder = string.Empty;
        public bool showAll = true;
        public bool showCombined = false;
        public bool sortCombined = true;
#if UNITY_EDITOR && UNITY_5_6_OR_NEWER
        private List<string> m_Names = null;
        private List<long> m_Values = null;

        private DynamicValue<string[]> m_DynamicNames = new DynamicValue<string[]>();

        private static GUIContent s_Content = new GUIContent();
        private static StringBuilder s_StringBuilder = new StringBuilder();
        private static int s_HashCode = "StackableDecorator.EnumMaskPopupAttribute".GetHashCode();
#endif
        public EnumMaskPopupAttribute()
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
            if (property.propertyType != SerializedPropertyType.Enum)
            {
                EditorGUI.LabelField(position, label.text, "Use with Enum.");
                return;
            }

            m_DynamicNames.UpdateAndCheckInitial(names, property);

            if (m_Names == null)
            {
                var exclude = this.exclude.Split(',');
                var type = m_FieldInfo.FieldType;
                type = type.IsArrayOrList() ? type.GetArrayOrListElementType() : type;
                m_Values = Enum.GetNames(type).Except(exclude).Select(n => Convert.ToInt64(Enum.Parse(type, n))).ToList();

                if (names == null)
                    m_Names = Enum.GetNames(type).Except(exclude).ToList();
                else
                {
                    var array = m_DynamicNames.GetValue();
                    m_Names = array == null ? names.Split(',').ToList() : array.ToList();
                }

                int index;
                while ((index = m_Values.IndexOf(0)) >= 0)
                {
                    if (names == null)
                        m_Names.RemoveAt(index);
                    m_Values.RemoveAt(index);
                }

                var length = Mathf.Min(m_Names.Count, m_Values.Count);
                m_Names = m_Names.Take(length).ToList();
                m_Values = m_Values.Take(length).ToList();
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