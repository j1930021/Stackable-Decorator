using StackableDecorator;
using UnityEngine;

public class IconSizeSample : MonoBehaviour
{
    [Heading(title = "GameObject", icon = "GameObject Icon")]
    [StackableField]
    public string IconSizeOff = "IconSize Off";

    [IconSize(16, 16, order = 1)]
    [Heading(title = "GameObject", icon = "GameObject Icon")]
    [StackableField]
    public string IconSize = "IconSize 16";
}