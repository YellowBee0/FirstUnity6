using System;

namespace YBFramework.Component
{
    [Serializable]
    public sealed class ConnectedDelegatePortData : ConnectedPortData
    {
        public bool IsExplicitCast;
    }
}