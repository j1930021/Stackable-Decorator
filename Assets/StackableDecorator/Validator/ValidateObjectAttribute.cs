using UnityEngine;
using System.Linq;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StackableDecorator
{
    public abstract class ValidateObjectAttribute : ValidateAttribute
    {
#if UNITY_EDITOR
        private DynamicValue<UnityEngine.Object> m_ObjectReference = null;
#endif

#if UNITY_EDITOR
        protected abstract bool OnCheckObject(UnityEngine.Object obj);

        private DynamicValue<UnityEngine.Object> GetObjectReference()
        {
            if (m_ObjectReference == null)
                m_ObjectReference = new DynamicValue<Object>("$", m_SerializedProperty);
            m_ObjectReference.Update(m_SerializedProperty);
            return m_ObjectReference;
        }

        protected override string OnProcessMessage(string message)
        {
            var type = m_SerializedProperty.type.Substring(6, m_SerializedProperty.type.Length - 7);
            var name = string.Join(", ", GetFailObjects(GetObjectReference()).Select(obj => obj.name).ToArray());
            message = message.Replace("%1", m_SerializedProperty.displayName);
            message = message.Replace("%2", type);
            message = message.Replace("%3", name);
            return message;
        }

        protected UnityEngine.Object[] GetFailObjects(DynamicValue<UnityEngine.Object> objectReference)
        {
            return objectReference.GetValues().Where(obj => obj != null && !OnCheckObject(obj)).Distinct().ToArray();
        }

        public override bool ValidateProperty(SerializedProperty property)
        {
            m_SerializedProperty = property;
            return GetObjectReference().GetValues().All(obj => OnCheckObject(obj));
        }

        public override float GetHeight(SerializedProperty property, GUIContent label, float height)
        {
            if (property.propertyType != SerializedPropertyType.ObjectReference) return height;
            return base.GetHeight(property, label, height);
        }

        public override bool BeforeGUI(ref Rect position, ref SerializedProperty property, ref GUIContent label, ref bool includeChildren, bool visible)
        {
            if (property.propertyType != SerializedPropertyType.ObjectReference) return visible;
            return base.BeforeGUI(ref position, ref property, ref label, ref includeChildren, visible);
        }
#endif
    }
}