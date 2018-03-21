using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StackableDecorator
{
    public class AnimatorParameterPopupAttribute : StackableFieldAttribute
    {
        public bool floatType = true;
        public bool intType = true;
        public bool boolType = true;
        public bool triggerType = true;
        public string exclude = string.Empty;
        public string placeHolder = string.Empty;
#if UNITY_EDITOR
        private string m_Animator = null;
        private string[] m_Exclude = null;

        private DynamicValue<UnityEngine.Object> m_DynamicAnimator = new DynamicValue<UnityEngine.Object>();

        private static GUIStyle s_Style = null;
        private static List<string> s_Parameters = new List<string>();
#endif
        public AnimatorParameterPopupAttribute()
        {
#if UNITY_EDITOR
#endif
        }

        public AnimatorParameterPopupAttribute(string animator)
        {
#if UNITY_EDITOR
            m_Animator = animator;
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

            if (m_Exclude == null)
                m_Exclude = exclude.Split(',');

            IEnumerable<Animator> animators = Enumerable.Empty<Animator>();
            if (m_Animator != null)
            {
                m_DynamicAnimator.UpdateAndCheckInitial(m_Animator, property);
                animators = m_DynamicAnimator.GetValues().Cast<Animator>();
            }
            else if (property.serializedObject.targetObject is MonoBehaviour)
            {
                animators = property.serializedObject.targetObjects.Cast<MonoBehaviour>().Select(m => m.GetComponent<Animator>());
            }
            var animatorControllers = animators.Select(a => a == null ? null : a.runtimeAnimatorController).Distinct();

            if (!animatorControllers.Any() || animatorControllers.Any(a => a == null))
            {
                EditorGUI.LabelField(position, label.text, "Load animator failed.");
                return;
            }
            else if (animatorControllers.Where(c => c != null).Count() != 1)
            {
                EditorGUI.LabelField(position, label.text, "Multiple animator is not supported.");
                return;
            }

            var animator = animators.First(a => a != null && a.runtimeAnimatorController != null);
            s_Parameters.Clear();
            foreach (var parameter in animator.parameters.Distinct())
            {
                if (parameter.type == AnimatorControllerParameterType.Float && !floatType) continue;
                if (parameter.type == AnimatorControllerParameterType.Int && !intType) continue;
                if (parameter.type == AnimatorControllerParameterType.Bool && !boolType) continue;
                if (parameter.type == AnimatorControllerParameterType.Trigger && !triggerType) continue;
                if (m_Exclude.Contains(parameter.name)) continue;
                s_Parameters.Add(parameter.name);
            }
            var parameters = s_Parameters.ToArray();

            int selected = ArrayUtility.IndexOf(parameters, property.stringValue);
            label = EditorGUI.BeginProperty(position, label, property);
            var value = EditorGUI.Popup(position, label.text, selected, parameters);
            if (value < 0 || value >= parameters.Length)
            {
                if (!property.hasMultipleDifferentValues)
                    EditorGUI.LabelField(position, " ", placeHolder, s_Style);
            }
            else if (value != selected)
                property.stringValue = parameters[value];
            EditorGUI.EndProperty();
        }
#endif
    }
}