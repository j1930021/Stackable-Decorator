using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StackableDecorator
{
    public class AsImageAttribute : StackableFieldAttribute
    {
        public float width = -1;
        public float height = -1;
        public string sizeGetter = null;
#if UNITY_EDITOR
        private DynamicValue<Vector2> m_DynamicSize = null;
        private Dictionary<string, Vector2> m_SizeCache = new Dictionary<string, Vector2>();
#endif
        public AsImageAttribute()
        {
#if UNITY_EDITOR
#endif
        }
#if UNITY_EDITOR
        private void Update()
        {
            if (sizeGetter == null) return;
            if (m_DynamicSize == null)
                m_DynamicSize = new DynamicValue<Vector2>(sizeGetter, m_SerializedProperty, new Vector2(width, height));
            m_DynamicSize.Update(m_SerializedProperty);
            var size = m_DynamicSize.GetValue();
            width = size.x;
            height = size.y;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label, bool includeChildren)
        {
            Update();
            m_SizeCache[property.propertyPath] = new Vector2(width, height);
            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label, bool includeChildren)
        {
            if (property.propertyType != SerializedPropertyType.ObjectReference || !(property.objectReferenceValue is Texture2D))
            {
                EditorGUI.LabelField(position, label.text, "Use with Texture2D.");
                return;
            }

            var size = m_SizeCache.Get(property.propertyPath, new Vector2(-1, -1));
            var tex = property.objectReferenceValue as Texture2D;
            position.width = size.x == -1 ? tex.width : size.x;
            position.height = size.y == -1 ? tex.height : size.y;
            GUI.DrawTexture(position, tex);
        }
#endif
    }
}