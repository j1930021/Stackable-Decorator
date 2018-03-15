using StackableDecorator;
using UnityEngine;

public class EnumPopupSample : MonoBehaviour
{
    public enum Alignment { Left, Center, Right }
    public enum ColorMask { R = 1, G = 2, B = 4, A = 8, RGB = R | G | B, RGBA = RGB | A }
    public enum Axis { X = 1, Y = 2, Z = 4 }

    private string[] names { get { return new[] { "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight" }; } set { } }
    private int[] values { get { return new[] { 1, 2, 3, 4, 5, 6, 7, 8 }; } set { } }

    [Heading(title = "Enum Popup")]
    [EnumPopup]
    public Alignment alignment;

    [EnumPopup(exclude = "RGB,RGBA", placeHolder = "Please select a channel.")]
    public ColorMask color1;

    [EnumPopup(names = "Red,Green,Blue,Alpha", placeHolder = "Please select a channel.")]
    public ColorMask color2;

    [EnumPopup(placeHolder = "Please select an axis.")]
    public Axis axis;

    [EnumPopup]
    public EventModifiers key1;

    [EnumPopup(exclude = "None", placeHolder = "Please select modifiers key.")]
    public EventModifiers key2;

    [Heading(top: 8, title = "Enum Mask Popup")]
    [EnumMaskPopup]
    public ColorMask mask1;

    [EnumMaskPopup(showAll = false, showCombined = true)]
    public ColorMask mask2;

    [EnumMaskPopup(names = "Shift,Alt,Ctrl,Command")]
    public EventModifiers key3;

    [Heading(top: 8)]
    [DropdownValue("#values", names = "#names", placeHolder = "Please select value.")]
    public int dropdownValue;

    [DropdownMask("#names", "#values", sortCombined = false, showCombined = true)]
    public int dropdownMask;

    [LayerMaskPopup]
    public LayerMask layerMask;
}
