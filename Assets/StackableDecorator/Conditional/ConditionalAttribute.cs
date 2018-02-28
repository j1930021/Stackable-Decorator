using UnityEngine;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StackableDecorator
{
    public abstract class ConditionalAttribute : StackableDecoratorAttribute
    {
        public bool inverted = false;
#if UNITY_EDITOR
        private bool? m_Static = null;
        private string m_Condition;
        private DynamicValue<bool> m_DynamicCondition = null;
#endif
        public ConditionalAttribute(bool condition)
        {
#if UNITY_EDITOR
            m_Static = condition;
#endif
        }
        public ConditionalAttribute(string condition)
        {
#if UNITY_EDITOR
            m_Condition = condition;
#endif
        }
#if UNITY_EDITOR
        protected DynamicValue<bool> GetCondition()
        {
            if (m_DynamicCondition == null)
            {
                if (m_Static == null)
                    m_DynamicCondition = new DynamicValue<bool>(m_Condition, m_SerializedProperty);
                else
                    m_DynamicCondition = new DynamicValue<bool>(m_Static.Value);
            }
            m_DynamicCondition.Update(m_SerializedProperty);
            return m_DynamicCondition;
        }

        protected bool MatchAll()
        {
            var check = inverted ? false : true;
            if (!m_SerializedProperty.hasMultipleDifferentValues)
                return GetCondition().GetValue() == check;
            return GetCondition().GetValues().All(value => value == check);
        }

        protected bool MatchAny()
        {
            var check = inverted ? false : true;
            if (!m_SerializedProperty.hasMultipleDifferentValues)
                return GetCondition().GetValue() == check;
            return GetCondition().GetValues().Any(value => value == check);
        }
#endif
    }
}