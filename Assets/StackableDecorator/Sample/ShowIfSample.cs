using StackableDecorator;
using UnityEngine;

public class ShowIfSample : MonoBehaviour
{
    public bool show1;
    public int show2;

    [Heading(height = 8, order = 1)]
    [ShowIf("$show1")]
    [StackableField]
    public string showIf1 = "Show if Show1 is true";

    [ShowIf("$show1", inverted = true)]
    [StackableField]
    public string showIf1b = "Show if Show1 is false";

    [ShowIf("#Visible")]
    [StackableField]
    public string showIf2 = "Show if Show2 is positive";

    [ShowIf("#Visible", order = 1)]
    [ShowIf("$show1", disable = false)]
    [StackableField]
    public string enableIf3 = "ShowIf1 or ShowIf2";

    [ShowIf("#Visible", order = 1)]
    [ShowIf("$show1", enable = false)]
    [StackableField]
    public string enableIf4 = "ShowIf1 and ShowIf2";

    public bool Visible()
    {
        return show2 >= 0;
    }
}