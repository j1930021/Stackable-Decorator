using StackableDecorator;
using System;
using UnityEngine;

public class HorizontalGroupSample : MonoBehaviour
{
    [HorizontalGroup("Group 1", 2, -1, 65, 65)]
    [StackableField]
    public string group1a;
    [InGroup("Group 1", order = 1)]
    [Label(-1, title = "B")]
    [StackableField]
    public string group1b;
    [InGroup("Group 1", order = 1)]
    [Label(-1, title = "C")]
    [StackableField]
    public string group1c;

    [HorizontalGroup("Group 2", false, "group2b,group2c", order = 1)]
    [Label(25, title = "A"), StackableField]
    public string group2a;
    [InGroup("Group 2", order = 1)]
    [Label(25, title = "B"), StackableField]
    public string group2b;
    [InGroup("Group 2", order = 1)]
    [Label(25, title = "C"), StackableField]
    public string group2c;

    [HorizontalGroup("Group 3", true, ".group3c", 0, prefix = true)]
    [StackableField]
    public Children group3;

    [Serializable]
    public class Children
    {
        [Label(0), StackableField]
        public string group3a;
        [Label(0), StackableField]
        public string group3b;
        [Label(0), StackableField]
        public string group3c;
        [Label(0), StackableField]
        public string group3d;
    }
}