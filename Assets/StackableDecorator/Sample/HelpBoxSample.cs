using StackableDecorator;
using UnityEngine;

public class HelpBoxSample : MonoBehaviour
{
    [HelpBox("Helpbox message on top.", below = false, messageType = MessageType.None)]
    [StackableField]
    public string HelpBox1;

    [HelpBox("Helpbox information.", messageType = MessageType.Info)]
    [StackableField]
    public string HelpBox2;

    [HelpBox("Helpbox warning.", messageType = MessageType.Warning)]
    [StackableField]
    public string HelpBox3;

    [HelpBox("Helpbox error.", messageType = MessageType.Error)]
    [StackableField]
    public string HelpBox4;
}