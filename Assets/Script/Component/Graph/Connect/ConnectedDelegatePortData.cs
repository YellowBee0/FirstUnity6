using System;

namespace YBFramework.Component
{
    [Serializable]
    public sealed class ConnectedDelegatePortData : ConnectedPortData
    {
        public static readonly ConnectedDelegatePortData Empty = new()
        {
            NodeID = -1,
            PortID = -1
        };

        public bool IsExplicitCast;
    }
}