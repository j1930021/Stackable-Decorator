using StackableDecorator;
using UnityEngine;

#if !UNITY_2018_1_OR_NEWER && UNITY_EDITOR
using UnityEditor;
[CanEditMultipleObjects]
[CustomEditor(typeof(ImageSample))]
public class ImageSampleEditor : Editor { }
#endif

public class ImageSample : MonoBehaviour
{
    [Heading(height = 8, order = 1)]
    [Image(image = "GameObject Icon")]
    [StackableField]
    public string image1 = "Using internal resource";

    [Heading(height = 8, order = 1)]
    [Image(image = "Assets/StackableDecorator/Sample/logo.png", alignment = TextAlignment.Right)]
    [StackableField]
    public string image2 = "Using file path";

    [Heading(height = 8, order = 1)]
    [Image(image = "31929cbaa6b12f441b2d2f5d23cbb8ce", GUID = true, alignment = TextAlignment.Center)]
    [StackableField]
    public string image3 = "Using GUID";

    [Heading(height = 8, order = 1)]
    [Image(texture = "$texture")]
    [StackableField]
    public string image4 = "Using texture";

    public Texture2D texture;

    [Heading(height = 8, order = 1)]
    [SideImage(image = "GameObject Icon", onLeft = true, height = 1)]
    [StackableField]
    public string side1;

    [Heading(height = 8, order = 1)]
    [SideImage(texture = "$texture", height = 48)]
    [StackableField]
    public string side2;
}