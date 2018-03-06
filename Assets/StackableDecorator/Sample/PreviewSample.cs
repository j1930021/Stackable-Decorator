using StackableDecorator;
using UnityEngine;

#if !UNITY_2018_1_OR_NEWER && UNITY_EDITOR
using UnityEditor;
[CanEditMultipleObjects]
[CustomEditor(typeof(PreviewSample))]
public class PreviewSampleEditor : Editor { }
#endif

public class PreviewSample : MonoBehaviour
{
    [Preview]
    [Expandable]
    public GameObject prefab;

    [Preview]
    [Expandable]
    public Material material;

    [Preview]
    [Expandable]
    public Texture texture;
}