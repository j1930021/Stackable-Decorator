using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StackableDecorator
{
    public class AssetOnlyAttribute : ValidateObjectAttribute
    {
#if UNITY_EDITOR
#endif
        public AssetOnlyAttribute()
        {
#if UNITY_EDITOR
            m_Message = "%2 %3 is not an asset.";
#endif
        }

        public AssetOnlyAttribute(string message)
        {
#if UNITY_EDITOR
            m_Message = message;
#endif
        }
#if UNITY_EDITOR
        protected override bool OnCheckObject(UnityEngine.Object obj)
        {
            return obj == null || EditorUtility.IsPersistent(obj);
        }
#endif
    }
}