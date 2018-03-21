using UnityEngine;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StackableDecorator
{
    public class InputAxisPopupAttribute : StackableFieldAttribute
    {
        public bool keyOrMouseButton = true;
        public bool mouseMovement = true;
        public bool joystickAxis = true;
        public bool negativeButton = true;
        public string exclude = string.Empty;
        public string placeHolder = string.Empty;
#if UNITY_EDITOR
        private string[] m_Exclude = null;

        private static GUIStyle s_Style = null;
        private static SerializedObject s_SerializedObject = null;
        private static List<string> s_Axes = new List<string>();
#endif
        public InputAxisPopupAttribute()
        {
#if UNITY_EDITOR
#endif
        }
#if UNITY_EDITOR
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label, bool includeChildren)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label, bool includeChildren)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.LabelField(position, label.text, "Use with String.");
                return;
            }

            if (s_Style == null)
            {
                s_Style = new GUIStyle(EditorStyles.popup);
                s_Style.normal.background = null;
            }

            if (s_SerializedObject == null)
            {
                var obj = AssetDatabase.LoadMainAssetAtPath("ProjectSettings/InputManager.asset");
                if (obj == null)
                    obj = Resources.FindObjectsOfTypeAll<UnityEngine.Object>().FirstOrDefault(o => o.name.Equals("InputManager"));
                if (obj != null)
                    s_SerializedObject = new SerializedObject(obj);
            }

            if (s_SerializedObject == null)
            {
                EditorGUI.LabelField(position, label.text, "Load InputManager failed.");
                return;
            }

            if (m_Exclude == null)
                m_Exclude = exclude.Split(',');

            s_Axes.Clear();
            foreach (SerializedProperty prop in s_SerializedObject.FindProperty("m_Axes"))
            {
                var type = prop.FindPropertyRelative("type");
                if (type.intValue == 0 && !keyOrMouseButton) continue;
                if (type.intValue == 1 && !mouseMovement) continue;
                if (type.intValue == 2 && !joystickAxis) continue;
                if (m_Exclude.Contains(prop.displayName)) continue;
                if (!negativeButton)
                {
                    if (prop.FindPropertyRelative("negativeButton").stringValue != string.Empty) continue;
                    if (prop.FindPropertyRelative("altNegativeButton").stringValue != string.Empty) continue;
                }
                s_Axes.Add(prop.displayName);
            }
            var axes = s_Axes.Distinct().ToArray();

            int selected = ArrayUtility.IndexOf(axes, property.stringValue);
            label = EditorGUI.BeginProperty(position, label, property);
            var value = EditorGUI.Popup(position, label.text, selected, axes);
            if (value < 0 || value >= axes.Length)
            {
                if (!property.hasMultipleDifferentValues)
                    EditorGUI.LabelField(position, " ", placeHolder, s_Style);
            }
            else if (value != selected)
                property.stringValue = axes[value];
            EditorGUI.EndProperty();
        }
#endif
    }
}