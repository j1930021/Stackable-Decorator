using StackableDecorator;
using UnityEngine;

public class EnumButtonSample : MonoBehaviour
{
    public enum Alignment { Left, Center, Right }
    public enum ColorMask { R = 1, G = 2, B = 4, A = 8, RGB = R | G | B, RGBA = RGB | A }
    public enum Axis { X = 1, Y = 2, Z = 4 }

    [HorizontalGroup("Main", false, "key", -1, 80, order = 1)]
    [Group("Left", 3)]
    [EnumButton]
    public Alignment alignment;

    [InGroup("Left")]
    [EnumButton(exclude = "RGB,RGBA")]
    public ColorMask color;

    [InGroup("Left")]
    [EnumMaskButton(all = false, column = 3)]
    public ColorMask colorMask;

    [InGroup("Left")]
    [EnumMaskButton(styles = "toggle")]
    public Axis axis;

    [InGroup("Main", order = 1)]
    [Label(0)]
    [EnumButton(column = 1, vOffset = -3)]
    public EventModifiers key;
}