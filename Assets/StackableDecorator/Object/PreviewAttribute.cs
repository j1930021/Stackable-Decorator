using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StackableDecorator
{
    public class PreviewAttribute : StackableDecoratorAttribute, INoCacheInspectorGUI
    {
        public bool indented = true;
        public bool always = false;
        public float height = 100;
#if UNITY_EDITOR
        private bool m_Visible;
        private Rect m_Position;

        private static Editor m_PreviewEditor;
#endif
        public PreviewAttribute()
        {
#if UNITY_EDITOR
#endif
        }
#if UNITY_EDITOR
        public override float GetHeight(SerializedProperty property, GUIContent label, float height)
        {
            if (!IsVisible()) return height;
            if (property.propertyType != SerializedPropertyType.ObjectReference) return height;
            if (property.objectReferenceValue == null)
            {
                Object.DestroyImmediate(m_PreviewEditor);
                m_PreviewEditor = null;
                return height;
            }
            if (!always && !property.isExpanded) return height;

            Editor.CreateCachedEditor(property.objectReferenceValue, null, ref m_PreviewEditor);
            if (m_PreviewEditor != null && m_PreviewEditor.HasPreviewGUI())
                height += this.height + 2;

            return height;
        }

        public override bool BeforeGUI(ref Rect position, ref SerializedProperty property, ref GUIContent label, ref bool includeChildren, bool visible)
        {
            m_Visible = visible;
            if (!IsVisible()) return visible;
            if (!visible) return false;
            if (property.propertyType != SerializedPropertyType.ObjectReference) return visible;
            if (m_PreviewEditor == null || !m_PreviewEditor.HasPreviewGUI()) return visible;
            if (!always && !property.isExpanded) return visible;

            m_Position = position.indent(indented);
            m_Position.yMin = m_Position.yMax - this.height;
            position.height -= this.height + 2; ;
            return true;
        }

        public override void AfterGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!m_Visible) return;
            if (m_PreviewEditor == null || !m_PreviewEditor.HasPreviewGUI()) return;
            if (!always && !property.isExpanded) return;

            m_PreviewEditor.DrawPreview(m_Position);
        }
#endif
    }
}