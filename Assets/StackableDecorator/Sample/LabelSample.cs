using StackableDecorator;
using UnityEngine;

public class LabelSample : MonoBehaviour
{
    [Label(title = "Title")]
    [StackableField]
    public string label1 = "Change Title";

    [Label(title = "GameObject", icon = "GameObject Icon", tooltip = "Tooltip")]
    [StackableField]
    public string label2 = "Change Title, Icon and Tooltip";

    [Label(100)]
    [StackableField]
    public string label3 = "Change Width";

    [Label(0)]
    [StackableField]
    public string label4 = "Hide Label";

    [Label(-1)]
    [StackableField]
    public string label5 = "Auto Width";

    [Label(-1, title = "Auto Width")]
    [StackableField]
    public string label6 = "Auto Width with title";
}