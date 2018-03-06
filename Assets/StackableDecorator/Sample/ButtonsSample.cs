using StackableDecorator;
using UnityEngine;

public class ButtonsSample : MonoBehaviour
{
    [Heading(height = 8, order = 1)]
    [Buttons(titles = "Button1,Button2,Button3", actions = "Button1,Button2,Button3", below = true)]
    [StackableField]
    public string button1;

    [Heading(height = 8, order = 2)]
    [IconSize(16, 16, order = 1)]
    [Buttons(titles = "GameObject,Prefab,Scene", icons = "GameObject Icon,PrefabNormal Icon,SceneAsset Icon",
        actions = "Button1,Button2,Button3", alignment = TextAlignment.Center)]
    [StackableField]
    public string button2;

    public void Button1() { button1 = button2 = "GameObject"; }
    public void Button2() { button1 = button2 = "Prefab"; }
    public void Button3() { button1 = button2 = "Scene"; }

    [Heading(height = 8, order = 1)]
    [SideButtons(titles = "+,-", actions = "Inc,Dec", onLeft = true, height = 1)]
    [StackableField]
    public int num1;

    [Heading(height = 8, order = 1)]
    [SideButtons(titles = ",", icons = "ol plus,ol minus", actions = "Inc,Dec")]
    [StackableField]
    public int num2;

    public void Inc() { num2 = ++num1; }
    public void Dec() { num2 = --num1; }
}