using StackableDecorator;
using UnityEngine;

public class Sample : MonoBehaviour
{
    [Heading(title = "Heading", order = 3)]
    [Color(0.5f, 0.5f, 1f, 1, order = 2)]
    [Box(2, 2, 2, 2)]
    [StackableField]
    public string field;
}