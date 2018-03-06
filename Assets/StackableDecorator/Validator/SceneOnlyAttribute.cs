using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StackableDecorator
{
    public class SceneOnlyAttribute : ValidateObjectAttribute
    {
#if UNITY_EDITOR
#endif
        public SceneOnlyAttribute()
        {
#if UNITY_EDITOR
            m_Message = "%2 %3 is not a scene object.";
#endif
        }

        public SceneOnlyAttribute(string message)
        {
#if UNITY_EDITOR
            m_Message = message;
#endif
        }
#if UNITY_EDITOR
        protected override bool OnCheckObject(UnityEngine.Object obj)
        {
            return obj == null || !EditorUtility.IsPersistent(obj);
        }
#endif
    }
}