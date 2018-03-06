using UnityEngine;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
using System.Collections.Generic;
#if UNITY_2017_1_OR_NEWER
using UnityEditor.Experimental.AssetImporters;
#endif
#endif

namespace StackableDecorator
{
    public class PopupEditorAttribute : StyledDecoratorAttribute
    {
        public float width = 16;
        public float height = 16;
#if UNITY_EDITOR
        protected override string m_defaultStyle { get { return "StaticDropdown"; } }

        private Dictionary<string, float> m_Height = new Dictionary<string, float>();
#endif
        public PopupEditorAttribute()
        {
#if UNITY_EDITOR
#endif
        }
#if UNITY_EDITOR
        public override float GetHeight(SerializedProperty property, GUIContent label, float height)
        {
            if (!IsVisible()) return height;
            m_Height[property.propertyPath] = height;
            var h = this.height < 0 ? m_ContentSize.y : this.height;
            height = Mathf.Max(h, height);
            return height;
        }

        public override bool BeforeGUI(ref Rect position, ref SerializedProperty property, ref GUIContent label, ref bool includeChildren, bool visible)
        {
            if (!IsVisible()) return visible;
            if (!visible) return false;
            if (Event.current.type == EventType.Layout) return visible;
            if (property.propertyType != SerializedPropertyType.ObjectReference) return visible;

            var w = width < 0 ? m_ContentSize.x : width;
            var h = height < 0 ? m_ContentSize.y : height;
            var rect = position.WidthFromRight(w).Height(h);
            using (new EditorGUI.DisabledScope(property.objectReferenceValue == null))
                if (GUI.Button(rect, m_Content, m_Style))
                    EditorPopup.Show(rect, property.objectReferenceValue, position.width);
            position.width -= w + 4;
            position.height = m_Height.Get(property.propertyPath, 16);
            return true;
        }

        public class EditorPopup : PopupWindowContent
        {
            public enum ColorType { iconTint, highlight, textColor }

            private float m_Width;
            private float m_Height = 1;
            private Editor m_Editor;
            private AssetImporter m_AssetImporter;
            private Editor m_AssetEditor;
            private bool m_ShowImportedObject = false;

            public static void Show(Rect rect, Object obj, float width)
            {
                var popup = new EditorPopup();
                popup.m_Width = width;
                popup.m_Editor = Editor.CreateEditor(obj);
                if (popup.m_Editor is MaterialEditor)
                {
                    var prop = typeof(MaterialEditor).GetProperty("forceVisible", BindingFlags.Instance | BindingFlags.NonPublic);
                    prop.SetValue(popup.m_Editor, true, null);
                }
                if (EditorUtility.IsPersistent(obj))
                {
                    var path = AssetDatabase.GetAssetPath(obj);
                    if (AssetDatabase.IsSubAsset(obj))
                        path += "/" + obj.name;
                    popup.m_AssetImporter = AssetImporter.GetAtPath(path);
                    var objs = Selection.objects;
                    popup.m_AssetEditor = Editor.CreateEditor(popup.m_AssetImporter);
                    if (popup.m_AssetEditor != null)
                    {
                        PropertyInfo prop;
#if UNITY_2017_1_OR_NEWER
                        if (popup.m_AssetEditor is AssetImporterEditor)
                        {
                            prop = typeof(AssetImporterEditor).GetProperty("assetEditor", BindingFlags.Instance | BindingFlags.NonPublic);
                            prop.SetValue(popup.m_AssetEditor, popup.m_Editor, null);
                            popup.m_ShowImportedObject = (popup.m_AssetEditor as AssetImporterEditor).showImportedObject;
                        }
                        else
                            popup.m_AssetImporter = null;
#else
                        var type = typeof(Editor).Assembly.GetType("UnityEditor.AssetImporterInspector");
                        if (type.IsAssignableFrom(popup.m_AssetEditor.GetType()))
                        {
                            prop = type.GetProperty("assetEditor", BindingFlags.Instance | BindingFlags.NonPublic);
                            prop.SetValue(popup.m_AssetEditor, popup.m_Editor, null);
                            prop = type.GetProperty("showImportedObject", BindingFlags.Instance | BindingFlags.NonPublic);
                            popup.m_ShowImportedObject = (bool)prop.GetValue(popup.m_AssetEditor, null);
                        }
                        else
                            popup.m_AssetImporter = null;
#endif
                    }
                    else
                        popup.m_AssetImporter = null;
                    Selection.objects = objs;
                }
                PopupWindow.Show(rect, popup);
            }

            public override Vector2 GetWindowSize()
            {
                return new Vector2(m_Width, m_Height);
            }

            public override void OnClose()
            {
                Object.DestroyImmediate(m_Editor);
                Object.DestroyImmediate(m_AssetEditor);
                m_Editor = null;
                m_AssetEditor = null;
            }

            public override void OnGUI(Rect rect)
            {
                var indentLevel = EditorGUI.indentLevel;
                var labelWidth = EditorGUIUtility.labelWidth;
                EditorGUI.indentLevel = 0;
                EditorGUIUtility.labelWidth = 0;
                EditorGUIUtility.wideMode = (m_Width > 330f);

                m_Editor.serializedObject.Update();
                if (m_AssetEditor != null)
                    m_AssetEditor.serializedObject.Update();
                GUILayout.BeginArea(rect);
                var r = EditorGUILayout.BeginVertical();
                if (m_AssetImporter != null)
                {
                    m_AssetEditor.serializedObject.Update();
                    m_AssetEditor.DrawHeader();
                    EditorGUIUtility.hierarchyMode = true;
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(14);
                    EditorGUILayout.BeginVertical();
                    m_AssetEditor.OnInspectorGUI();
                    if (m_ShowImportedObject)
                        m_Editor.OnInspectorGUI();
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                    m_Editor.DrawHeader();
                    EditorGUIUtility.hierarchyMode = true;
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(14);
                    EditorGUILayout.BeginVertical();
                    m_Editor.OnInspectorGUI();
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndHorizontal();
                }
                GUILayout.Space(3);
                EditorGUILayout.EndVertical();
                GUILayout.EndArea();

                m_Editor.serializedObject.ApplyModifiedProperties();
                if (m_AssetEditor != null)
                    m_AssetEditor.serializedObject.ApplyModifiedProperties();

                if (Event.current.type == EventType.Repaint)
                {
                    if (m_Height != r.height)
                    {
                        m_Height = r.height;
                        editorWindow.Repaint();
                    }
                }
                EditorGUI.indentLevel = indentLevel;
                EditorGUIUtility.labelWidth = labelWidth;
            }
        }
#endif
    }
}