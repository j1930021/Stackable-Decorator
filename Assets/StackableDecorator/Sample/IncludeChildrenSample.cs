using StackableDecorator;
using System;
using UnityEngine;

public class IncludeChildrenSample : MonoBehaviour
{
    public On includeChildrenOn;

    [IncludeChildren(false)]
    [StackableField]
    public Off includeChildrenOff;

    [Serializable]
    public class On
    {
        public string includeChildren = "IncludeChildren On";
    }

    [Serializable]
    public class Off
    {
        public string includeChildren = "IncludeChildren Off";
    }
}