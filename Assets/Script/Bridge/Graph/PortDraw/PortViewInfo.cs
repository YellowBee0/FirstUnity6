#if UNITY_EDITOR
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace YBFramework.MyEditor
{
    public struct PortViewInfo
    {
        public string Name;

        public Direction Direction;

        public Port.Capacity Capacity;

        public Color Color;

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