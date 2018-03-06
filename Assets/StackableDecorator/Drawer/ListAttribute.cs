using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

namespace StackableDecorator
{
    public class ListAttribute : StackableFieldAttribute
    {
        public bool expandable = false;
#if UNITY_EDITOR
        private string m_List = string.Empty;
        private Data m_CurrentData;

        private Dictionary<string, Data> m_Data = new Dictionary<string, Data>();

        private class Data
        {
            public int arraySize;
            public List<float> propertyHeights = new List<float>();
            public SerializedProperty listProperty = null;
            public ReorderableList reorderableList = null;

            public int lastIndex;
            public SerializedProperty lastElement = null;
        }
#endif
        public ListAttribute()
        {
#if UNITY_EDITOR
#endif
        }

        public ListAttribute(string list)
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

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label, bool includeChildren)
        {
            var data = m_Data.Get(property.propertyPath);
            data.listProperty = GetProperty();
            if (data.listProperty == null) return EditorGUIUtility.singleLineHeight;

            data.arraySize = GetArraySize(data.listProperty);
            data.propertyHeights.Clear();
            if (data.reorderableList == null)
            {
                data.reorderableList = new ReorderableList(property.serializedObject, data.listProperty);
                data.reorderableList.drawHeaderCallback += DrawHeaderCallback;
                data.reorderableList.elementHeightCallback += ElementHeightCallback;
                data.reorderableList.drawElementCallback += DrawElementCallback;
            }
            data.reorderableList.serializedProperty = data.listProperty;

            float height = 38;
            if (expandable && !m_SerializedProperty.isExpanded)
                height = EditorGUIUtility.singleLineHeight;
            else
            {
                var element = data.listProperty.FindPropertyRelative("Array.data[0]");
                for (int i = 0; i < data.arraySize; i++)
                {
                    var propertyHeight = EditorGUI.GetPropertyHeight(element, null, true);
                    data.propertyHeights.Add(propertyHeight);
                    element.NextVisible(false);

                    height += propertyHeight + 2;
                }
                if (data.arraySize == 0)
                    height += 20;
            }
            return height;
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

            m_CurrentData = data;
            m_CurrentData.lastIndex = 0;
            m_CurrentData.lastElement = m_CurrentData.listProperty.FindPropertyRelative("Array.data[0]");

            if (!expandable || m_SerializedProperty.isExpanded)
                data.reorderableList.DoList(position);
            else
                DrawHeaderCallback(position.Shrink(6, 0, 0, 0));
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

        private void DrawHeaderCallback(Rect rect)
        {
            if (!expandable)
            {
                GUI.Label(rect, m_SerializedProperty.displayName);
                return;
            }
            rect.xMin -= 6;
            var label = EditorGUI.BeginProperty(rect, null, m_SerializedProperty);
            var result = EditorGUI.Foldout(rect, m_SerializedProperty.isExpanded, label, true, EditorStyles.foldout);
            if (result != m_SerializedProperty.isExpanded)
                m_SerializedProperty.isExpanded = result;
            EditorGUI.EndProperty();
        }

        private float ElementHeightCallback(int index)
        {
            //return EditorGUI.GetPropertyHeight(m_CurrentData.listProperty.GetArrayElementAtIndex(index), null, true) + 2;
            return m_CurrentData.propertyHeights[index] + 2;
        }

        private void DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (index == m_CurrentData.lastIndex + 1)
                m_CurrentData.lastElement.NextVisible(false);
            else if (index != m_CurrentData.lastIndex)
                m_CurrentData.lastElement = m_CurrentData.listProperty.GetArrayElementAtIndex(index);
            m_CurrentData.lastIndex = index;

            var hierarchyMode = EditorGUIUtility.hierarchyMode;
            int indentLevel = EditorGUI.indentLevel;
            EditorGUIUtility.hierarchyMode = false;
            EditorGUI.indentLevel = 0;
            EditorGUI.PropertyField(rect, m_CurrentData.lastElement, null, true);
            EditorGUIUtility.hierarchyMode = hierarchyMode;
            EditorGUI.indentLevel = indentLevel;
        }
#endif
    }
}