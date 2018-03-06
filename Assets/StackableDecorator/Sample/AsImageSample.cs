using StackableDecorator;
using UnityEngine;

#if !UNITY_2018_1_OR_NEWER && UNITY_EDITOR
using UnityEditor;
[CanEditMultipleObjects]
[CustomEditor(typeof(AsImageSample))]
public class AsImageSampleEditor : Editor { }
#endif

public class AsImageSample : MonoBehaviour
{
    [OnDragDrop(order = 1)]
    [HelpBox("Drop image here.", below = false, messageType = StackableDecorator.MessageType.None)]
    [AsImage]
    public Texture2D asImage;
}