using StackableDecorator;
using UnityEngine;

#if !UNITY_2018_1_OR_NEWER && UNITY_EDITOR
using UnityEditor;
[CanEditMultipleObjects]
[CustomEditor(typeof(EventSample))]
public class EventSampleEditor : Editor { }
#endif

public class EventSample : MonoBehaviour
{
    public bool showHelpBox { get; set; }

    [HelpBox("Value Changed", "#showHelpBox", order = 2)]
    [OnValueChanged("OnValueChanged", order = 1)]
    [Group("OnValueChanged", 2)]
    [StackableField]
    public string data1 = "Data1";
    [InGroup("OnValueChanged"), Slider(0, 100)]
    public int data2;
    [InGroup("OnValueChanged"), StackableField]
    public Color data3;

    [OnClick("#OnClick", order = 1)]
    [Box(2, 2, 2, 2)]
    [AsString]
    public string onClick = "Click to hide the help box.";

    public void OnClick()
    {
        showHelpBox = false;
    }

    public void OnValueChanged()
    {
        showHelpBox = true;
    }

    [Heading(height = 8, order = 2)]
    [OnDragDrop(autoDrop = false, after = true, order = 1)]
    [HelpBox("Drag image out.", below = false, messageType = StackableDecorator.MessageType.None)]
    [StackableField]
    public Texture2D dragOnly;

    [Heading(height = 8, order = 2)]
    [OnDragDrop(order = 1)]
    [HelpBox("Drag image out or drop image here.", below = false, messageType = StackableDecorator.MessageType.None)]
    [AsImage]
    public Texture2D dragDrop;

    [Heading(height = 8, order = 2)]
    [OnDragDrop(autoDrag = false, order = 1)]
    [HelpBox("Drop image here.", below = false, messageType = StackableDecorator.MessageType.None)]
    [AsImage]
    public Texture2D dropOnly;
}