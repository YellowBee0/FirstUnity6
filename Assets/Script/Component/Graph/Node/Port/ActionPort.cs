using System;
using System.Reflection;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
#if DEBUG
using YBFramework.MyEditor;
#endif

namespace YBFramework.Component
{
    [Serializable]
    public sealed class ActionPort : DelegatePort<Action>
    {
        [SerializeField] private ConnectedDelegatePortData[] m_ConnectedPortData;

        public void Invoke()
        {
#if DEBUG
            GraphDebugHelper.RecordInvoke(Node.Graph, this, m_ConnectedPortData);
#endif
            m_Delegate?.Invoke();
        }

        public override object DynamicInvoke(params object[] args)
        {
            Invoke();
            return null;
        }

        public override ConnectedPortData ConnectedPortDataIterator(int index)
        {
            if (m_ConnectedPortData != null)
            {
                return index >= m_ConnectedPortData.Length ? null : m_ConnectedPortData[index];
            }
            return null;
        }
#if UNITY_EDITOR
        public override string GetConnectTip()
        {
            return "no parameter and return value";
        }

        protected override bool CanConnect(BasePort other, out bool isExplicitCast)
        {
            isExplicitCast = false;
            if (other is MethodPort methodPort)
            {
                ParameterInfo[] methodPortParameterInfos = methodPort.GetParameters();
                if (methodPortParameterInfos.Length == 0)
                {
                    if (methodPort.GetReturnType() != typeof(void))
                    {
                        isExplicitCast = true;
                    }
                    return true;
                }
            }
            return false;
        }

        public override bool Connect(int nodeID, BasePort other)
        {
            if (CanConnect(other, out bool isExplicitCast))
            {
                ConnectedDelegatePortData connectedPortData;
                int index;
                if (m_ConnectedPortData != null)
                {
                    if (PortViewInfo.Capacity == Port.Capacity.Single)
                    {
                        connectedPortData = m_ConnectedPortData[0];
                        index = 0;
                    }
                    else
                    {
                        if (IPortConnectionSource.FindConnectedPortData(m_ConnectedPortData, nodeID, other.GetID()) != null)
                        {
                            return false;
                        }
                        connectedPortData = new ConnectedDelegatePortData();
                        index = m_ConnectedPortData.Length;
                        Array.Resize(ref m_ConnectedPortData, m_ConnectedPortData.Length + 1);
                    }
                }
                else
                {
                    connectedPortData = new ConnectedDelegatePortData();
                    index = 0;
                    m_ConnectedPortData = new ConnectedDelegatePortData[1];
                }
                connectedPortData.NodeID = nodeID;
                connectedPortData.PortID = other.GetID();
                connectedPortData.IsExplicitCast = isExplicitCast;
                m_ConnectedPortData[index] = connectedPortData;
                return true;
            }
            return false;
        }

        public override bool Disconnect(int nodeID, BasePort other)
        {
            for (int i = 0; i < m_ConnectedPortData.Length; i++)
            {
                ConnectedDelegatePortData connectedPortData = m_ConnectedPortData[i];
                if (connectedPortData.NodeID == nodeID && connectedPortData.PortID == other.GetID())
                {
                    (m_ConnectedPortData[i], m_ConnectedPortData[^1]) = (m_ConnectedPortData[^1], m_ConnectedPortData[i]);
                    Array.Resize(ref m_ConnectedPortData, m_ConnectedPortData.Length - 1);
                    return true;
                }
            }
            return false;
        }
#endif
    }
}