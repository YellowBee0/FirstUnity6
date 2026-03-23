#if UNITY_EDITOR
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace YBFramework.MyEditor
{
    public struct PortViewInfo
    {
        public readonly string Name;

        public readonly Direction Direction;

        public readonly Port.Capacity Capacity;

        public readonly Color Color;

        public PortViewInfo(string name, Direction direction, Port.Capacity capacity, Color color)
        {
            Name = name;
            Direction = direction;
            Capacity = capacity;
            Color = color;
        }
    }
}
#endif