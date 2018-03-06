using StackableDecorator;
using UnityEngine;

public class Logo : MonoBehaviour
{
    [Heading(top: 8, order = 7)]
    [HorizontalGroup("Logo", 0, 98, order = 6)]
    [Box(4, 4, 4, 4, style = "flow node 1 on", order = 5)]
    [Heading(top: 2, bottom: 6, title = "Stackable", width = -1,
        style = "WarningOverlay", order = 4)]
    [Color(1f, 1f, 0.5f, 1, order = 3)]
    [Box(4, 4, 4, 4, style = "flow overlay box", order = 2)]
    [Label(-1)]
    [StackableField]
    public bool Decorator = true;
}