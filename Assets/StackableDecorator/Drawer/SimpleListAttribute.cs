using UnityEngine;
using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StackableDecorator
{
    public class SimpleListAttribute : StackableFieldAttribute, INoCacheInspectorGUI
    {
        public float spacing = 2;
        public float fixedHeight = -1;
        public float maxHeight = -1;

        public string maxHeightGetter = null;
#if UNITY_EDITOR
        private string m_List = string.Empty;

        private DynamicValue<float> m_DynamicMaxHeight = null;
        private Dictionary<string, Data> m_Data = new Dictionary<string, Data>();

        private class Data
        {
            public float listHeight;
            public List<float> propertyHeights = new List<float>();
            public float maxHeight;
            public int arraySize;
            public SerializedProperty listProperty;
            public Vector2 scroll;
        }
#endif
        public SimpleListAttribute()
        {
#if UNITY_EDITOR
#endif
        }

        public SimpleListAttribute(string list)
        {
#if UNITY_EDITOR
            m_List = list;
#endif
        }
#if UNITY_EDITOR
        private SerializedProperty GetProperty()
        {
            SerializedProperty result;
            if (!string.IsNullOrEmpty(m_List))
                result = m_SerializedProperty.GetProperties(m_List.Yield())[0];
            else
            {
                result = m_SerializedProperty.Copy();
                if (!result.Next(true))
                    result = null;
            }
            if (result != null && !result.isArray)
                result = null;
            return result;
        }

        private void Update()
        {
            if (m_DynamicMaxHeight == null)
                m_DynamicMaxHeight = new DynamicValue<float>(maxHeightGetter, m_SerializedProperty, maxHeight);
            m_DynamicMaxHeight.Update(m_SerializedProperty);
            if (maxHeightGetter != null)
                maxHeight = m_DynamicMaxHeight.GetValue();
        }

        private int GetArraySize(SerializedProperty property)
        {
            if (property == null) return 0;
            if (!property.hasMultipleDifferentValues)
                return property.arraySize;

            int result = property.arraySize;
            foreach (var obj in property.serializedObject.targetObjects)
            {
                var so = new SerializedObject(obj);
                var prop = so.FindProperty(property.propertyPath);
                result = Math.Min(prop.arraySize, result);
            }
            return result;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label, bool includeChildren)
        {
            var data = m_Data.Get(property.propertyPath);
            data.listProperty = GetProperty();
            if (data.listProperty == null) return EditorGUIUtility.singleLineHeight;

            Update();

            data.arraySize = GetArraySize(data.listProperty);
            data.propertyHeights.Clear();
            data.maxHeight = maxHeight;

            float result = 0;
            var element = data.listProperty.FindPropertyRelative("Array.data[0]");
            for (int i = 0; i < data.arraySize; i++)
            {
                //var propertyHeight = StackableDecorateGUI.GetPropertyHeight(data.serializedProperty.GetArrayElementAtIndex(i), null, true);
                var propertyHeight = EditorGUI.GetPropertyHeight(element, null, true);
                data.propertyHeights.Add(propertyHeight);
                element.NextVisible(false);

                var height = fixedHeight >= 0 ? fixedHeight : propertyHeight;
                if (height < 0) height = 0;
                if (result > 0 && height > 0) result += spacing;
                result += height;
            }

            data.listHeight = result;
            if (maxHeight >= 0)
                result = Mathf.Min(result, maxHeight);
            return result;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label, bool includeChildren)
        {
            if (Event.current.type == EventType.Layout) return;

            var data = m_Data.Get(property.propertyPath);
            if (data.listProperty == null)
            {
                EditorGUI.LabelField(position, label.text, "Use with list.");
                return;
            }

            var realHeight = data.listHeight;
            var view = new Rect(position);
            view.height = realHeight;
            if (realHeight > position.height)
                view.width -= 16;
            var realWidth = view.width;

            if (realHeight > position.height)
                data.scroll = GUI.BeginScrollView(position, data.scroll, view);
            var rect = new Rect(position);
            rect.width = realWidth;

            var element = data.listProperty.FindPropertyRelative("Array.data[0]");
            for (int i = 0; i < data.arraySize; i++)
            {
                //var element = data.serializedProperty.GetArrayElementAtIndex(i);
                rect.height = fixedHeight >= 0 ? fixedHeight : data.propertyHeights[i];
                EditorGUI.PropertyField(rect.Height(data.propertyHeights[i]), element, null, true);
                rect = rect.MoveDown(spacing);
                element.NextVisible(false);
            }
            if (realHeight > position.height)
                GUI.EndScrollView();
        }
#endif
    }
}