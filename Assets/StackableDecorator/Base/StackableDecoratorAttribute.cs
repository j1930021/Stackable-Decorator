using System;
using UnityEngine;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StackableDecorator
{
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = true)]
    public abstract class StackableDecoratorAttribute : Attribute
    {
        public bool visible = true;
        public int order = 0;
        public string byPass = string.Empty;
#if UNITY_EDITOR
        protected SerializedProperty m_SerializedProperty = null;
        protected FieldInfo m_FieldInfo = null;

        private DynamicValue<bool> m_ByPass = null;

        public void SetSerializedProperty(SerializedProperty property)
        {
            m_SerializedProperty = property.Copy();
        }

        public void SetFieldInfo(FieldInfo fieldInfo)
        {
            m_FieldInfo = fieldInfo;
        }

        public bool IsVisible()
        {
            if (m_ByPass == null)
                m_ByPass = new DynamicValue<bool>(byPass, m_SerializedProperty);
            m_ByPass.Update(m_SerializedProperty);
            if (!string.IsNullOrEmpty(byPass))
                if (m_ByPass.GetValue()) return false;
            return visible;
        }

        public virtual bool BeforeGetHeight(ref SerializedProperty property, ref GUIContent label, ref bool includeChildren) { return true; }
        public virtual void AfterGetHeight(SerializedProperty property, GUIContent label, bool includeChildren) { }

        public virtual float GetHeight(SerializedProperty property, GUIContent label, float height)
        {
            return height;
        }

        public virtual bool BeforeGUI(ref Rect position, ref SerializedProperty property, ref GUIContent label, ref bool includeChildren, bool visible) { return true; }
        public virtual void AfterGUI(Rect position, SerializedProperty property, GUIContent label) { }
#endif
    }
}