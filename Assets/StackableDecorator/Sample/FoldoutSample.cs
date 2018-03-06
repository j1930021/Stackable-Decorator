using StackableDecorator;
using UnityEngine;

public class FoldoutSample : MonoBehaviour
{
    [Foldout(title = "Foldout")]
    [StackableField]
    public string foldout1 = "Simple Foldout";

    [Foldout(title = "Foldout Group", order = 2)]
    [Group("Foldout Group", 1)]
    [StackableField]
    public string foldout2a = "Foldout with group";
    [InGroup("Foldout Group")]
    [StackableField]
    public string foldout2b;

    [Box(0, 0, 4, 6, style = "label", order = 3)]
    [Foldout(title = "Foldout with style", hierarchyMode = false, indentChildren = false, 
        style2 = "flow overlay header lower left", order = 2)]
    [Box(2, 4, 2, 4, style = "flow overlay box", order = 1)]
    [Group("Foldout with style", 2)]
    [StackableField]
    public string foldout3a = "Foldout with style";
    [InGroup("Foldout with style"), StackableField]
    public string foldout3b;
    [InGroup("Foldout with style"), StackableField]
    public string foldout3c;
}