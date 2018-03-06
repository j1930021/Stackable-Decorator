using UnityEngine;
using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StackableDecorator
{
    public class SimpleGridAttribute : StackableFieldAttribute, INoCacheInspectorGUI
    {
        public int column = 4;
        public float cellWidth = -1;
        public float cellHeight = 16;
        public float spacing = 2;
        public float maxHeight = -1;

        public string columnGetter = null;
        public string cellSizeGetter = null;
        public string maxHeightGetter = null;
#if UNITY_EDITOR
        private string m_List = string.Empty;

        private DynamicValue<int> m_DynamicColumn = null;
        private DynamicValue<Vector2> m_DynamicSize = null;
        private DynamicValue<float> m_DynamicMaxHeight = null;
        private Dictionary<string, Data> m_Data = new Dictionary<string, Data>();

        private class Data
        {
            public int row;
            public int column;
            public float cellWidth;
            public float cellHeight;
            public float maxHeight;
            public int arraySize;
            public SerializedProperty serializedProperty;
            public Vector2 scroll;
        }
#endif
        public SimpleGridAttribute()
        {
#if UNITY_EDITOR
#endif
        }

        public SimpleGridAttribute(string list)
        {
#if UNITY_EDITOR
            m_List = list;
#endif
        }
#if UNITY_EDITOR
        private SerializedProperty GetProperty()
        {
            SerializedProperty result, prop;
            if (!string.IsNullOrEmpty(m_List))
                result = m_SerializedProperty.GetProperties(m_List.Yield())[0];
            else
            {
                prop = m_SerializedProperty.Copy();
                if (!prop.Next(true))
                    prop = null;
                result = prop;
            }
            if (result != null && !result.isArray)
                result = null;
            return result;
        }

        private void Update()
        {
            if (m_DynamicColumn == null)
                m_DynamicColumn = new DynamicValue<int>(columnGetter, m_SerializedProperty, column);
            m_DynamicColumn.Update(m_SerializedProperty);
            if (columnGetter != null)
                column = m_DynamicColumn.GetValue();
            column = Mathf.Max(1, column);

            if (m_DynamicSize == null)
                m_DynamicSize = new DynamicValue<Vector2>(cellSizeGetter, m_SerializedProperty, new Vector2(cellWidth, cellHeight));
            m_DynamicSize.Update(m_SerializedProperty);
            if (cellSizeGetter != null)
            {
                var size = m_DynamicSize.GetValue();
                cellWidth = size.x;
                cellHeight = size.y;
            }

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
            data.serializedProperty = GetProperty();
            if (data.serializedProperty == null) return EditorGUIUtility.singleLineHeight;

            Update();

            data.arraySize = GetArraySize(data.serializedProperty);
            var row = data.arraySize / column;
            if (data.arraySize % column != 0) row++;
            data.row = row;
            data.column = column;
            data.cellWidth = cellWidth;
            data.cellHeight = cellHeight;
            data.maxHeight = maxHeight;

            var height = row * cellHeight + (row - 1) * spacing;
            if (maxHeight >= 0)
                height = Mathf.Min(height, maxHeight);

            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label, bool includeChildren)
        {
            if (Event.current.type == EventType.Layout) return;

            var data = m_Data.Get(property.propertyPath);
            if (data.serializedProperty == null)
            {
                EditorGUI.LabelField(position, label.text, "Use with list.");
                return;
            }

            var realHeight = data.row * data.cellHeight;
            var view = new Rect(position);
            view.height = realHeight;
            if (realHeight > position.height)
                view.width -= 16;
            var realWidth = view.width;

            if (realHeight > position.height)
                data.scroll = GUI.BeginScrollView(position, data.scroll, view);

            int index = 0;
            var element = data.serializedProperty.FindPropertyRelative("Array.data[0]");
            var grid = view.GridDistribute(data.cellWidth < 0 ? realWidth / data.column : data.cellWidth, data.cellHeight, data.row, data.column, spacing);
            if (data.arraySize > 0)
                foreach (var cell in grid)
                {
                    //var element = data.serializedProperty.GetArrayElementAtIndex(index);
                    var h = EditorGUI.GetPropertyHeight(element, null, true);
                    EditorGUI.PropertyField(cell.Height(h), element, null, true);
                    element.NextVisible(false);
                    index++;
                    if (index >= data.arraySize) break;
                }
            if (realHeight > position.height)
                GUI.EndScrollView();
        }
#endif
    }
}