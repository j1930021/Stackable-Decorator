using StackableDecorator;
using UnityEngine;

public class DrawerSample : MonoBehaviour
{
    [Box]
    [StackableField]
    public string box1 = "Simple Box";

    [TextField(placeHolder = "Please enter the text.")]
    public string textField;

    [TextField(3, placeHolder = "Please enter the text.")]
    public string textField2;

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

    [ColorField(showEyedropper = false)]
    public Color color;

    [CurveField(1, 0, 1, -1, -1, 1, 1)]
    public AnimationCurve curve;

    [ProgressBar(100, decimalPlaces = 0)]
    public int progress = 53;

    [ProgressBar(22, 88, showLabel = false, prefix = true)]
    public int progress2 = 39;
}