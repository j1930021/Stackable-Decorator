using UnityEngine;
using UnityEngine.Profiling;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StackableDecorator
{
    public class LabelAttribute : StyledDecoratorAttribute
    {
#if UNITY_EDITOR
        protected override string m_defaultStyle { get { return "label"; } }

        private float m_LabelWidth = -2;
        private float m_SavedLabelWidth;
#endif
        public LabelAttribute()
        {
#if UNITY_EDITOR
#endif
        }

        public LabelAttribute(float width)
        {
#if UNITY_EDITOR
            m_LabelWidth = width;
#endif
        }
#if UNITY_EDITOR
        private void ProcessLabel(ref GUIContent label)
        {
            var content = m_Content;
            if (title == null)
                content.text = label.text;
            if (icon == null)
                content.image = label.image;
            if (tooltip == null)
                content.tooltip = label.tooltip;
            label = content;
        }

        public override bool BeforeGetHeight(ref SerializedProperty property, ref GUIContent label, ref bool includeChildren)
        {
            if (!IsVisible()) return true;
            ProcessLabel(ref label);
            return true;
        }

        public override bool BeforeGUI(ref Rect position, ref SerializedProperty property, ref GUIContent label, ref bool includeChildren, bool visible)
        {
            m_SavedLabelWidth = EditorGUIUtility.labelWidth;
            if (!IsVisible()) return visible;

            ProcessLabel(ref label);

            if (m_LabelWidth == 0)
                label = GUIContent.none;
            else if (m_LabelWidth == -2)
                EditorGUIUtility.labelWidth = 0;
            else if (m_LabelWidth == -1)
                EditorGUIUtility.labelWidth = m_Style.CalcSize(label).x + 2 + (position.indent().x - position.x);
            else if (m_LabelWidth > 0 && m_LabelWidth <= 1)
                EditorGUIUtility.labelWidth = m_LabelWidth * position.width;
            else
                EditorGUIUtility.labelWidth = m_LabelWidth;
            return visible;
        }

        public override void AfterGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUIUtility.labelWidth = m_SavedLabelWidth;
        }
#endif
    }
}