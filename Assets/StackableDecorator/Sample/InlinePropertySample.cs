using StackableDecorator;
using UnityEngine;

public class InlinePropertySample : MonoBehaviour
{
    [InlineProperty]
    [Expandable]
    public Transform trans;

    [InlineProperty]
    [Expandable]
    public Material material;

    [InlineProperty]
    [Expandable]
    public Object obj;
}