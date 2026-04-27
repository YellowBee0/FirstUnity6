using System;
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

        public override bool CanConnect(BasePort other)
        {
            if (other is MethodPort methodPort)
            {
                if (methodPort.GetParameters().Length == 0)
                {
                    return true;
                }
            }
            return false;
        }

        public override void Connect(int nodeID, BasePort other)
        {
            MethodPort methodPort = (MethodPort)other;
            bool isExplicitCast = methodPort.GetReturnType() != typeof(void);
            if (IPortConnectionSource.FindConnectedPortData(m_ConnectedPortData, nodeID, other.ID) == null)
            {
                int index = 0;
                if (m_ConnectedPortData != null)
                {
                    index = m_ConnectedPortData.Length;
                    Array.Resize(ref m_ConnectedPortData, index + 1);
                }
                else
                {
                    m_ConnectedPortData = new ConnectedDelegatePortData[1];
                }
                m_ConnectedPortData[index] = new ConnectedDelegatePortData
                {
                    NodeID = nodeID,
                    PortID = other.ID,
                    IsExplicitCast = isExplicitCast
                };
                ConnectedCount++;
                other.ConnectedCount++;
            }
        }

        public override void Disconnect(int nodeID, BasePort other)
        {
            for (int i = 0; i < m_ConnectedPortData.Length; i++)
            {
                ConnectedDelegatePortData connectedPortData = m_ConnectedPortData[i];
                if (connectedPortData.NodeID == nodeID && connectedPortData.PortID == other.ID)
                {
                    (m_ConnectedPortData[i], m_ConnectedPortData[^1]) = (m_ConnectedPortData[^1], m_ConnectedPortData[i]);
                    Array.Resize(ref m_ConnectedPortData, m_ConnectedPortData.Length - 1);
                    ConnectedCount--;
                    other.ConnectedCount--;
                    return;
                }
            }
        }

        public override void InitPortViewInfo(string name, string fieldName, Direction direction, Port.Capacity capacity, Color color)
        {
            direction = Direction.Output;
            color = Color.red;
            base.InitPortViewInfo(name, fieldName, direction, capacity, color);
        }
#endif
    }
}