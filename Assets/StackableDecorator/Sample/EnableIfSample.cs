using StackableDecorator;
using UnityEngine;

public class EnableIfSample : MonoBehaviour
{
    public bool enable1;
    public int enable2;

    [Heading(height = 8, order = 1)]
    [EnableIf("$enable1")]
    [StackableField]
    public string enableIf1 = "Enable if Enable1 is true";

    [EnableIf("$enable1", inverted = true)]
    [StackableField]
    public string enableIf1b = "Enable if Enable1 is false";

    [EnableIf("#Active")]
    [StackableField]
    public string enableIf2 = "Enable if Enable2 is positive";

    [EnableIf("#Active", order = 1)]
    [EnableIf("$enable1", disable = false)]
    [StackableField]
    public string enableIf3 = "EnableIf1 or EnableIf2";

    [EnableIf("#Active", order = 1)]
    [EnableIf("$enable1", enable = false)]
    [StackableField]
    public string enableIf4 = "EnableIf1 and EnableIf2";

    public bool Active()
    {
        return enable2 >= 0;
    }
}