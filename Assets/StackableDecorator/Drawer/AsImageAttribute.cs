using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StackableDecorator
{
    public class AsImageAttribute : StackableFieldAttribute, INoCacheInspectorGUI
    {
        public float width = -1;
        public float height = -1;
        public string sizeGetter = null;
#if UNITY_EDITOR
        private DynamicValue<Vector2> m_DynamicSize = null;

        private Dictionary<string, Data> m_Data = new Dictionary<string, Data>();

        private class Data
        {
            public Texture2D image;
            public Vector2 size;
        }
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
            if (property.propertyType != SerializedPropertyType.ObjectReference || !(property.objectReferenceValue is Texture2D))
                return EditorGUIUtility.singleLineHeight;

            Update();
            var data = m_Data.Get(property.propertyPath);
            data.image = property.objectReferenceValue as Texture2D;
            data.size = new Vector2();
            data.size.x = width == -1 ? data.image.width : width;
            data.size.y = height == -1 ? data.image.height : height;
            return data.size.y;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label, bool includeChildren)
        {
            if (property.propertyType != SerializedPropertyType.ObjectReference || !(property.objectReferenceValue is Texture2D))
            {
                EditorGUI.LabelField(position, label.text, "Use with Texture2D.");
                return;
            }

            var data = m_Data.Get(property.propertyPath);
            position.size = data.size;
            GUI.DrawTexture(position, data.image);
        }
#endif
    }
}