using StackableDecorator;
using UnityEngine;

public class DrawerSample : MonoBehaviour
{
    [Box]
    [StackableField]
    public string box1 = "Simple Box";

    [Color(1, 0.5f, 0.5f, 1)]
    [Slider(0, 100)]
    public int slider = 0;

    [Color(0.5f, 0.5f, 1, 1)]
    [RangeSlider(0, 100)]
    public Vector2 rangeSlider = new Vector2(25, 50);

    [ToggleLeft]
    public bool toggleLeft;

    [Expandable]
    public string expandable;

    [LabelOnly]
    public string labelOnly;

    [Label(icon = "GameObject Icon")]
    [AsString(label = false, icon = true)]
    public string asString = "As String Sample";
}