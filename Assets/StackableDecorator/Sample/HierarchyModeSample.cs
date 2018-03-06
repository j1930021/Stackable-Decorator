using StackableDecorator;
using System;
using UnityEngine;

public class HierarchyModeSample : MonoBehaviour
{
    public On hierarchyModeOn;

    [HierarchyMode(false)]
    [StackableField]
    public Off hierarchyModeOff;

    [Serializable]
    public class On
    {
        public string hierarchyMode = "HierarchyMode On";
    }

    [Serializable]
    public class Off
    {
        public string hierarchyMode = "HierarchyMode Off";
    }
}