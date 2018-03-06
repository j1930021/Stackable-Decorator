using StackableDecorator;
using UnityEngine;

public class ColorSample : MonoBehaviour
{
    [Color(1, 0.5f, 0.5f, 1)]
    [StackableField]
    public string color1 = "Simple Color";

    [Color(0.5f, 0.5f, 1, 1, order = 1)]
    [Box(4, 4, 4, 4)]
    [StackableField]
    public string color2 = "Colored Box";
}