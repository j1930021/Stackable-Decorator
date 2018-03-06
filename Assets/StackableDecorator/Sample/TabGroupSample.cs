using StackableDecorator;
using System;
using UnityEngine;

public class TabGroupSample : MonoBehaviour
{
    [TabGroup("Tab Group", "Tab1,Tab2,Tab3", "group2a,group3", bottom: 2, order = 1)]
    [Group("Group 1", 2)]
    [StackableField]
    public string group1a = "Group with 2 following properties";
    [InGroup("Group 1")]
    [StackableField]
    public string group1b;
    [InGroup("Group 1")]
    [StackableField]
    public string group1c;

    [InGroup("Tab Group", order = 1)]
    [Group("Group 2", false, "group2b,group2c")]
    [StackableField]
    public string group2a = "Group with 2 selected properties";
    [InGroup("Group 2"), StackableField]
    public string group2b;
    [InGroup("Group 2"), StackableField]
    public string group2c;

    [InGroup("Tab Group", order = 1)]
    [Group("Group 3", true, ".group3c", 0)]
    [StackableField]
    public Children group3;

    [Serializable]
    public class Children
    {
        public string group3a = "Group with all children but group3c";
        public string group3b;
        public string group3c;
        public string group3d;
    }
}