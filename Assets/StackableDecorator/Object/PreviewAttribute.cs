using UnityEngine;
using System.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
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

        private static Dictionary<WeakReference, Editor> s_EditorRef = new Dictionary<WeakReference, Editor>();

        private Dictionary<string, Data> m_Data = new Dictionary<string, Data>();

        private class Data
        {
            public Editor previewEditor = null;
        }
#endif
        public PreviewAttribute()
        {
#if UNITY_EDITOR
#endif
        }
#if UNITY_EDITOR
        private static Func<object> s_PropertyHandlerCacheGetter = null;
        private static Func<object, object, object> s_GetHandlerFunc = null;

        private object GetHandler(SerializedProperty property)
        {
            if (s_PropertyHandlerCacheGetter == null)
            {
                var type = typeof(Editor).Assembly.GetType("UnityEditor.ScriptAttributeUtility");
                var method = type.GetProperty("propertyHandlerCache", BindingFlags.Static | BindingFlags.NonPublic).GetGetMethod(true);
                s_PropertyHandlerCacheGetter = method.MakeStaticFunc<Func<object>>();
                type = typeof(Editor).Assembly.GetType("UnityEditor.PropertyHandlerCache");
                method = type.GetMethod("GetHandler", BindingFlags.Instance | BindingFlags.NonPublic);
                s_GetHandlerFunc = method.MakeFuncGenericInput<Func<object, object, object>>();
            }
            var cache = s_PropertyHandlerCacheGetter();
            var handler = s_GetHandlerFunc(cache, property);
            return handler;
        }

        private void CleanUp(Editor editor)
        {
            WeakReference weak = null;
            var handler = GetHandler(m_SerializedProperty);
            foreach (var kv in s_EditorRef.ToList())
            {
                if (kv.Key.Target == null)
                {
                    UnityEngine.Object.DestroyImmediate(kv.Value);
                    s_EditorRef.Remove(kv.Key);
                }
                if (kv.Key.Target == handler)
                {
                    weak = kv.Key;
                    if (kv.Value != editor)
                    {
                        UnityEngine.Object.DestroyImmediate(kv.Value);
                        s_EditorRef[kv.Key] = editor;
                    }
                }
            }
            if (weak == null)
            {
                weak = new WeakReference(handler);
                s_EditorRef[weak] = editor;
            }
        }

        public override float GetHeight(SerializedProperty property, GUIContent label, float height)
        {
            if (!IsVisible()) return height;
            if (property.propertyType != SerializedPropertyType.ObjectReference) return height;
            if (property.objectReferenceValue == null) return height;
            if (!always && !property.isExpanded) return height;

            var data = m_Data.Get(property.propertyPath);
            if (data.previewEditor == null)
                data.previewEditor = Editor.CreateEditor(property.objectReferenceValue);
            CleanUp(data.previewEditor);
            if (data.previewEditor != null && data.previewEditor.HasPreviewGUI())
                height += this.height + 2;

            return height;
        }

        public override bool BeforeGUI(ref Rect position, ref SerializedProperty property, ref GUIContent label, ref bool includeChildren, bool visible)
        {
            m_Visible = visible;
            if (!IsVisible()) return visible;
            if (!visible) return false;
            if (Event.current.type == EventType.Layout) return visible;
            if (property.propertyType != SerializedPropertyType.ObjectReference) return visible;

            var data = m_Data.Get(property.propertyPath);
            if (data.previewEditor == null || !data.previewEditor.HasPreviewGUI()) return visible;
            if (!always && !property.isExpanded) return visible;

            m_Position = position.indent(indented);
            m_Position.yMin = m_Position.yMax - height;
            position.height -= height + 2; ;
            return true;
        }

        public override void AfterGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!m_Visible) return;
            if (Event.current.type == EventType.Layout) return;

            var data = m_Data.Get(property.propertyPath);
            if (data.previewEditor == null || !data.previewEditor.HasPreviewGUI()) return;
            if (!always && !property.isExpanded) return;

            data.previewEditor.DrawPreview(m_Position);
        }
#endif
    }
}