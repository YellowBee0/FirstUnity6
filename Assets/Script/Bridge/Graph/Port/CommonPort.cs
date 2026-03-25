#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace YBFramework.Component
{
    [Serializable]
    public sealed class CommonPort : BasePort, IPortConnectionSource
    {
        [SerializeField] private List<ConnectedPortData> m_ConnectedPortData;

        public void Connect(IReadOnlyList<BaseNode> nodes)
        {
        }

        public bool CanConnect(BasePort other)
        {
            return true;
        }

        public bool Connect(int nodeID, BasePort other)
        {
            if (m_ConnectedPortData != null)
            {
                if (m_ConnectedPortData.Count > 0)
                {
                    if (PortViewInfo.Capacity == Port.Capacity.Single)
                    {
                        m_ConnectedPortData[0] = new ConnectedPortData
                        {
                            NodeID = nodeID,
                            PortID = other.ID
                        };
                        ConnectedCount++;
                        other.ConnectedCount++;
                        return true;
                    }
                    if (IPortConnectionSource.FindConnectedPortData(m_ConnectedPortData, nodeID, other.ID) != null)
                    {
                        return false;
                    }
                }
            }
            else
            {
                m_ConnectedPortData = new List<ConnectedPortData>();
            }
            m_ConnectedPortData.Add(new ConnectedPortData
            {
                NodeID = nodeID,
                PortID = other.ID
            });
            ConnectedCount++;
            other.ConnectedCount++;
            return true;
        }

        public bool Disconnect(int nodeID, BasePort other)
        {
            for (int i = 0; i < m_ConnectedPortData.Count; i++)
            {
                if (m_ConnectedPortData[i].NodeID == nodeID && m_ConnectedPortData[i].PortID == other.ID)
                {
                    m_ConnectedPortData.RemoveAt(i);
                    ConnectedCount--;
                    other.ConnectedCount--;
                    return true;
                }
            }
            return false;
        }

        public ConnectedPortData ConnectedPortDataIterator(int index)
        {
            return index >= m_ConnectedPortData.Count ? null : m_ConnectedPortData[index];
        }

        public override string GetConnectTip()
        {
            return "help editor to show other port";
        }
    }
}
#endif