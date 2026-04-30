using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace YBFramework.Component
{
    [Serializable]
    public abstract class DelegatePort : BasePort, IPortConnectionSource
    {
        public abstract object DynamicInvoke(params object[] args);

        public abstract void Connect(BaseNode node, ConnectedPortData data);

        public abstract void Connect(IReadOnlyList<BaseNode> nodes);

        public abstract ConnectedPortData ConnectedPortDataIterator(int index);
#if UNITY_EDITOR
        public abstract bool CanConnect(BasePort other);

        public abstract void Connect(int nodeID, BasePort other);

        public abstract void Disconnect(int nodeID, BasePort other);

        public void CheckConnectValid(IReadOnlyList<BaseNode> nodes)
        {
            if (m_Capacity == Port.Capacity.Single && ConnectedCount > 1)
            {
                Debug.LogError($"port id {ID} is single connect,but connected multi count");
                return;
            }
            foreach (ConnectedPortData connectedPortData in ((IPortConnectionSource)this).GetConnectedPortDataEnumerable())
            {
                for (int i = 0; i < nodes.Count; i++)
                {
                    BaseNode node = nodes[i];
                    if (node.ID == connectedPortData.NodeID)
                    {
                        BasePort port = node.GetPort(connectedPortData.PortID);
                        if (port == null)
                        {
                            Debug.LogError($"port id {connectedPortData.PortID} in node id {connectedPortData.NodeID} not found");
                            break;
                        }
                        if (!CanConnect(port))
                        {
                            Debug.LogError($"port id {connectedPortData.PortID} in node id {connectedPortData.NodeID} can not connected");
                            break;
                        }
                    }
                }
            }
        }
#endif
    }

    public abstract class DelegatePort<TDelegate> : DelegatePort where TDelegate : Delegate
    {
        protected TDelegate m_Delegate;

        public override void Connect(BaseNode node, ConnectedPortData data)
        {
            if (data is ConnectedDelegatePortData connectedDelegatePortData)
            {
                if (node.GetPort(connectedDelegatePortData.PortID) is MethodPort connectedPort)
                {
                    Type delegateType = typeof(TDelegate);
                    m_Delegate = (TDelegate)Delegate.Combine(m_Delegate,
                        connectedDelegatePortData.IsExplicitCast
                            ? PortConnectHelper.WrapMethod(delegateType, connectedPort.GetMethodInfo(), node)
                            : connectedPort.GetMethodInfo().CreateDelegate(delegateType, node));
                }
            }
        }

        public override void Connect(IReadOnlyList<BaseNode> nodes)
        {
            Delegate @delegate = null;
            Type delegateType = typeof(TDelegate);
            foreach (ConnectedPortData connectedPortData in ((IPortConnectionSource)this).GetConnectedPortDataEnumerable())
            {
                ConnectedDelegatePortData connectedDelegatePortData = (ConnectedDelegatePortData)connectedPortData;
                for (int i = 0; i < nodes.Count; i++)
                {
                    BaseNode node = nodes[i];
                    if (node.ID == connectedDelegatePortData.NodeID)
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