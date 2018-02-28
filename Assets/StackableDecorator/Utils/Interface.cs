using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StackableDecorator
{
    public interface INoCacheInspectorGUI { }

    public interface IValidateProperty
    {
        MessageType messageType { get; }
#if UNITY_EDITOR
        bool ValidateProperty(SerializedProperty property);
        string GetMessage(SerializedProperty property);
#endif
    }
}