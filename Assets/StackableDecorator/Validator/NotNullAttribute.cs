using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StackableDecorator
{
    public class NotNullAttribute : ValidateObjectAttribute
    {
#if UNITY_EDITOR
#endif
        public NotNullAttribute()
        {
#if UNITY_EDITOR
            m_Message = "%1 should not be none.";
#endif
        }

        public NotNullAttribute(string message)
        {
#if UNITY_EDITOR
            m_Message = message;
#endif
        }
#if UNITY_EDITOR
        protected override bool OnCheckObject(UnityEngine.Object obj)
        {
            return obj != null;
        }
#endif
    }
}