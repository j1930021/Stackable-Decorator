using StackableDecorator;
using UnityEngine;

public class HeadingSample : MonoBehaviour
{
    [Heading(title = "Header")]
    [StackableField]
    public string heading1 = "Simple Heading";

    [Heading(title = "Footer", below = true)]
    [StackableField]
    public string heading2 = "Below";

    [Heading(4, 4, 4, 4, title = "Header", alignment = TextAlignment.Right)]
    [StackableField]
    public string heading3 = "Padding and right alignment";

    [Heading(4, 4, 4, 4, title = "Footer", below = true, alignment = TextAlignment.Center, style = "box")]
    [StackableField]
    public string heading4 = "Style, padding and center alignment";

    [Heading(title = "Header", icon = "console.infoicon.sml", width = 0.5f, height = 24, alignment = TextAlignment.Center, style = "box")]
    [StackableField]
    public string heading5 = "Icon, Width(%) and height";

    [Heading(4, 0, -12, 0, width = 1, height = 22, style = "PR Insertion")]
    [StackableField]
    public string heading6 = "Style and bottom padding -12";

    [Heading(-14, -5, 0, 2, width = 1, title = "Header", style = "box")]
    [StackableField]
    public string heading7 = "Negative padding";
}