using UnityEngine;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StackableDecorator
{
    public class ValidateValueAttribute : ValidateAttribute
    {
        public bool inverted = false;
#if UNITY_EDITOR
        private string m_Condition;
        private DynamicValue<bool> m_DynamicCondition = null;
#endif
        public ValidateValueAttribute(string message, string condition)
        {
#if UNITY_EDITOR
            m_Message = message;
            m_Condition = condition;
#endif
        }
#if UNITY_EDITOR
        protected DynamicValue<bool> GetCondition()
        {
            if (m_DynamicCondition == null)
                m_DynamicCondition = new DynamicValue<bool>(m_Condition, m_SerializedProperty);
            m_DynamicCondition.Update(m_SerializedProperty);
            return m_DynamicCondition;
        }

        public override bool ValidateProperty(SerializedProperty property)
        {
            m_SerializedProperty = property;
            var check = inverted ? false : true;
            if (!property.hasMultipleDifferentValues)
                return GetCondition().GetValue() == check;
            return GetCondition().GetValues().All(value => value == check);
        }

        protected override string OnProcessMessage(string message)
        {
            message = message.Replace("%1", m_SerializedProperty.displayName);
            return message;
        }
#endif
    }
}