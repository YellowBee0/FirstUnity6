using System;
using System.Collections.Generic;

namespace YBFramework.Component
{
    [Serializable]
    public abstract class DelegatePort : BasePort, IPortConnectionSource
    {
        public abstract object DynamicInvoke(params object[] args);

        public abstract void Connect(IReadOnlyList<BaseNode> nodes);

        public abstract ConnectedPortData ConnectedPortDataIterator(int index);
#if UNITY_EDITOR
        protected abstract bool CanConnect(BasePort other, out bool isExplicitCast);

        public bool CanConnect(BasePort other)
        {
            return CanConnect(other, out _);
        }

        public abstract bool Connect(int nodeID, BasePort other);

        public abstract bool Disconnect(int nodeID, BasePort other);
#endif
    }

    public abstract class DelegatePort<TDelegate> : DelegatePort where TDelegate : Delegate
    {
        protected TDelegate m_Delegate;

        public override void Connect(IReadOnlyList<BaseNode> nodes)
        {
            Delegate @delegate = null;
            Type delegateType = typeof(TDelegate);
            foreach (ConnectedPortData connectedPortData in ((IPortConnectionSource)this).GetConnectedPortDataEnumerator())
            {
                ConnectedDelegatePortData connectedDelegatePortData = (ConnectedDelegatePortData)connectedPortData;
                for (int j = 0; j < nodes.Count; j++)
                {
                    BaseNode node = nodes[j];
                    if (node.GetID() == connectedDelegatePortData.NodeID)
                    {
                        if (node.GetPort(connectedDelegatePortData.PortID) is MethodPort connectedPort)
                        {
                            @delegate = Delegate.Combine(m_Delegate,
                                connectedDelegatePortData.IsExplicitCast
                                    ? PortConnectHelper.WrapMethod(delegateType, connectedPort.GetMethodInfo(), node)
                                    : connectedPort.GetMethodInfo().CreateDelegate(delegateType, node));
                        }
                    }
                }
            }
            m_Delegate = (TDelegate)@delegate;
        }
    }
}