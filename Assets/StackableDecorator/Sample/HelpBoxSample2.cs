using StackableDecorator;
using UnityEngine;

public class HelpBoxSample2 : MonoBehaviour
{
    public bool show1;
    public int show2;

    [Heading(height = 8, order = 1)]
    [HelpBox("Show if Show1 is true", "$show1")]
    [StackableField]
    public string HelpBox1;

    [HelpBox("Show if Show1 is false", "$show1", inverted = true)]
    [StackableField]
    public string HelpBox1b;

    [HelpBox("Show if Show2 is positive", "#Visible")]
    [StackableField]
    public string HelpBox2;

    [HelpBox("Show if self is positive", "#CheckValue")]
    [StackableField]
    public int HelpBox2b = 0;

    public bool Visible()
    {
        return show2 >= 0;
    }

    public bool CheckValue(int value)
    {
        return value >= 0;
    }
}