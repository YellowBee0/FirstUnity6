#if UNITY_EDITOR
using System;

namespace YBFramework.MyEditor
{
    public sealed class NodeMenuAttribute : Attribute
    {
        public readonly GraphType GraphType;
        public readonly string NodeName;

        public NodeMenuAttribute(string nodeName, GraphType graphType)
        {
            NodeName = nodeName;
            GraphType = graphType;
        }
    }
}
#endif