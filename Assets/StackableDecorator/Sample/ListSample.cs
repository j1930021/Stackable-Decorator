using StackableDecorator;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ListSample : MonoBehaviour
{
    [List]
    [SerializeField]
    private DataList m_DataList;
    public List<Data> dataList { get { return m_DataList.list; } }

    [Heading(title = "Nested List")]
    [List]
    [SerializeField]
    public NestedList nestedList;

    [Heading(title = "SimpleList")]
    [SimpleList("m_DataList.list")]
    public string placeholder1;

    [Heading(top: 8, title = "SimpleGrid")]
    [SimpleGrid("m_DataList.list", column = 2, cellHeight = 16)]
    public string placeholder2;

    [Serializable]
    public class NestedList
    {
        [List(expandable = true)]
        public List<DataList> list;
    }

    [Serializable]
    public class DataList
    {
        [Label(-1, order = 1)]
        [HorizontalGroup("info", true, "", 0, 15, -1, 50, prefix = true)]
        [StackableField]
        public List<Data> list;
    }

    [Serializable]
    public class Data
    {
        [Label(0), StackableField]
        public bool enable;
        [Label(0), Slider(0, 100, showField = false)]
        public int value;
        [Label(0), StackableField]
        public Color color;
    }
}