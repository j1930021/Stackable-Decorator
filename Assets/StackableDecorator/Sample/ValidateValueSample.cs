using StackableDecorator;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ValidateValueSample : MonoBehaviour
{
    public int referenceValue = 100;

    [ValidateValue("%1 cannot larger than Reference Value.", "#CheckValue")]
    [StackableField]
    public int validateValue1 = 0;

    [ValidateValue("%1 cannot larger than Reference Value.", "#CheckValue")]
    [StackableField]
    public int validateValue2 = 200;

    [ValidateValue("%1 require ValidateValueSample.", "#CheckObject")]
    [StackableField]
    public GameObject validateObject1;

    [ValidateValue("%1 require ValidateValueSample.", "#CheckObject")]
    [StackableField]
    public GameObject validateObject2;

    [ValidateValue("%1 require ValidateValueSample.", "#CheckObject")]
    [StackableField]
    public GameObject validateObject3;

    [ValidateValue("More than 2 elements.", "#CheckArray")]
    [StackableField]
    public int[] validateArray;

    public bool CheckValue(int value)
    {
        return value <= referenceValue;
    }

    public bool CheckObject(GameObject go)
    {
        return go != null && go.GetComponent<ValidateValueSample>() != null;
    }
#if UNITY_EDITOR
    public bool CheckArray(SerializedProperty property)
    {
        var path = property.propertyPath;
        var num = path.Substring(path.LastIndexOf('[') + 1).TrimEnd(']');
        int index;
        if (!int.TryParse(num, out index))
            return false;
        return index < 2;
    }
#endif
}