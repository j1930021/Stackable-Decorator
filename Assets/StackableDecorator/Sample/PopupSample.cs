using StackableDecorator;
using UnityEngine;

public class PopupSample : MonoBehaviour
{
    [TagPopup]
    public string tag1 = "Untagged";

    [TagPopup(placeHolder = "Please select a tag.", exclude = "Untagged")]
    public string tag2;

    [LayerPopup]
    public int layer1;

    [LayerPopup(placeHolder = "Please select a layer.", exclude = "Default")]
    public string layer2;

    [SortingLayerPopup]
    public int sortingLayer1;

    [SortingLayerPopup(placeHolder = "Please select a layer.", exclude = "Default")]
    public string sortingLayer2;

    [InputAxisPopup(placeHolder = "Please select a input.")]
    public string input1;

    [InputAxisPopup(placeHolder = "Please select a input.", mouseMovement = false, joystickAxis = false, negativeButton = false)]
    public string input2;

    [AnimatorParameterPopup(placeHolder = "Please select a parameter.")]
    public string parameter1;

    [AnimatorParameterPopup(placeHolder = "Please select a parameter.", triggerType = false)]
    public string parameter2;
}
