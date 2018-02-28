using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StackableDecorator
{
    public class ImageAttribute : StackableDecoratorAttribute, INoCacheInspectorGUI
    {
        public string image = string.Empty;
        public string texture = string.Empty;
        public float width = -1;
        public float height = -1;
        public bool GUID = false;
        public bool indented = true;
        public bool below = false;
        public TextAlignment alignment = TextAlignment.Left;
#if UNITY_EDITOR
        private float m_Left;
        private float m_Right;
        private float m_Top;
        private float m_Bottom;

        private DynamicValue<string> m_Image = null;
        private DynamicValue<UnityEngine.Object> m_Texture = null;
        private Dictionary<string, Data> m_Data = new Dictionary<string, Data>();

        private class Data
        {
            public Texture image = null;
            public Vector2 imageSize;
        }
#endif
        public ImageAttribute(float left = 0, float right = 0, float top = 0, float bottom = 0)
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
            data.image = GetImage();
            data.imageSize = Vector2.zero;
            if (data.image != null)
                data.imageSize = new Vector2(data.image.width, data.image.height);

            float image = this.height < 0 ? data.imageSize.y : this.height;
            image += m_Top + m_Bottom;
            if (image < 0) image = 0;
            return height + image;
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
                width = data.imageSize.x;
            else if (this.width <= 1)
                width = indent.width * this.width;
            else
                width = this.width;
            rect.width = width;
            rect.height = height < 0 ? data.imageSize.y : height;

            if (alignment == TextAlignment.Right)
            {
                rect.xMin = indent.xMax - rect.width;
                rect.xMax = indent.xMax;
            }
            else if (alignment == TextAlignment.Center)
            {
                rect.x += (indent.width - rect.width) / 2;
            }
            rect.xMin += m_Left;
            rect.xMax -= m_Right;

            if (below)
                rect.y = indent.yMax - rect.height - m_Top - m_Bottom;
            rect.y += m_Top;

            if (data.image != null)
                GUI.DrawTexture(rect, data.image);

            float image = rect.height + m_Top + m_Bottom;
            if (image < 0) image = 0;
            if (below)
                position.height -= image;
            else
                position.yMin += image;
            return true;
        }
#endif
    }
}