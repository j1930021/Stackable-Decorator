using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StackableDecorator
{
    public class SideImageAttribute : StyledDecoratorAttribute, INoCacheInspectorGUI
    {
        public string image = string.Empty;
        public string texture = string.Empty;
        public float width = -1;
        public float height = -1;
        public float margin = 2;
        public bool GUID = false;
        public bool indented = true;
        public bool onLeft = false;
        public bool reserveWidth = false;
        public TextAlignment alignment = TextAlignment.Left;
#if UNITY_EDITOR
        protected override string m_defaultStyle { get { return "label"; } }

        private float m_Left;
        private float m_Right;
        private float m_Top;
        private float m_Bottom;

        private DynamicValue<string> m_Image = null;
        private DynamicValue<UnityEngine.Object> m_Texture = null;
        private Dictionary<string, Data> m_Data = new Dictionary<string, Data>();

        private class Data
        {
            public float height;
            public float imageHeight;
            public Texture image = null;
            public Vector2 imageSize;
        }
#endif
        public SideImageAttribute(float left = 0, float right = 0, float top = 0, float bottom = 0)
        {
#if UNITY_EDITOR
            m_Left = left;
            m_Right = right;
            m_Top = top;
            m_Bottom = bottom;
#endif
        }
#if UNITY_EDITOR
        private Texture GetImage()
        {
            if (m_Image == null)
            {
                m_Image = new DynamicValue<string>(image, m_SerializedProperty);
                m_Texture = new DynamicValue<UnityEngine.Object>(texture, m_SerializedProperty);
            }
            m_Image.Update(m_SerializedProperty);
            m_Texture.Update(m_SerializedProperty);

            var obj = m_Texture.GetValue();
            if (obj != null && obj is Texture)
                return obj as Texture;

            var img = m_Image.GetValue();
            if (!string.IsNullOrEmpty(img))
            {
                var tex = GUID ? img.GUIDToImage() : img.GetImage();
                if (tex != null)
                    return tex;
            }

            return null;
        }

        public override float GetHeight(SerializedProperty property, GUIContent label, float height)
        {
            if (!IsVisible()) return height;

            var data = m_Data.Get(property.propertyPath);
            data.height = height;
            data.image = GetImage();
            data.imageSize = Vector2.zero;
            if (data.image != null)
                data.imageSize = new Vector2(data.image.width, data.image.height);

            if (this.height < 0)
                data.imageHeight = data.imageSize.y + m_Top + m_Bottom;
            else if (this.height <= 1)
                data.imageHeight = data.height * this.height;
            else
                data.imageHeight = this.height;
            height = Mathf.Max(data.height, data.imageHeight);

            return height;
        }

        public override bool BeforeGUI(ref Rect position, ref SerializedProperty property, ref GUIContent label, ref bool includeChildren, bool visible)
        {
            if (!IsVisible()) return visible;
            if (!visible) return false;

            var data = m_Data.Get(property.propertyPath);

            var indent = position.indent(indented);
            var rect = new Rect(indent);

            float width = 0;
            if (this.width < 0)
                width = data.imageSize.x * data.imageHeight / data.imageSize.y;
            else if (this.width <= 1)
                width = indent.width * this.width;
            else
                width = this.width;
            if (reserveWidth && this.width >= 0)
                width = indent.width - width;
            rect.width = width;
            rect.height = data.imageHeight;

            if (!onLeft)
            {
                rect.xMin = indent.xMax - rect.width;
                rect.xMax = indent.xMax;
            }
            GUI.Box(rect, GUIContent.none, m_Style);

            rect.xMin += m_Left;
            rect.xMax -= m_Right;

            rect.yMin += m_Top;
            rect.yMax -= m_Bottom;

            if (data.image != null)
                GUI.DrawTexture(rect, data.image);

            position.height = data.height;
            position.width -= rect.width + (onLeft ? m_Right : m_Left) + margin;
            if (onLeft)
                position.x += rect.width + m_Right + margin;
            return true;
        }
#endif
    }
}