using StackableDecorator;
using UnityEngine;

public class BoxSample : MonoBehaviour
{
    [Box]
    [StackableField]
    public string box1 = "Simple Box";

    [Box(4, 4, 4, 4)]
    [StackableField]
    public string box2 = "Padding 4";

    [Box(-4, -4, -4, -4)]
    [StackableField]
    public string box3 = "Padding -4";

    [Box(2, 2, 2, 2, style = "ShurikenEffectBg")]
    [StackableField]
    public string box4 = "Style";
}