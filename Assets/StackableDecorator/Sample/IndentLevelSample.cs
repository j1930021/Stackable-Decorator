using StackableDecorator;
using System;
using UnityEngine;

public class IndentLevelSample : MonoBehaviour
{
    public string indentLevel1 = "IndentLevel Off";

    [IndentLevel(1)]
    [StackableField]
    public string indentLevel2 = "IndentLevel +1";

    [IndentLevel(2)]
    [StackableField]
    public string indentLevel3 = "IndentLevel +2";

    public Children children;

    [Serializable]
    public class Children
    {
        public string indentLevel1 = "IndentLevel Off";

        [IndentLevel(-1)]
        [StackableField]
        public string indentLevel2 = "IndentLevel -1";

        [IndentLevel(-2)]
        [StackableField]
        public string indentLevel3 = "IndentLevel -2";

        [IndentLevel(0, absolute = true)]
        [StackableField]
        public string indentLevel4 = "IndentLevel 0 (absolute)";

        [IndentLevel(3, absolute = true)]
        [StackableField]
        public string indentLevel5 = "IndentLevel 3 (absolute)";
    }
}