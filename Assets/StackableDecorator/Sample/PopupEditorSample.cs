using StackableDecorator;
using UnityEngine;

public class PopupEditorSample : MonoBehaviour
{
    [PopupEditor]
    [StackableField]
    public Transform trans;

    [PopupEditor]
    [StackableField]
    public Material material;

    [PopupEditor(width = -1, title = "Popup", style = "minibutton")]
    [StackableField]
    public Object obj;
}