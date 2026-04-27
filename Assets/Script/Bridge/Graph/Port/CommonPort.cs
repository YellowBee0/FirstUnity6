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

        public void Connect(BaseNode node, ConnectedPortData data)
        {
        }

        public void Connect(IReadOnlyList<BaseNode> nodes)
        {
        }

        public bool CanConnect(BasePort other)
        {
            return true;
        }

        public void Connect(int nodeID, BasePort other)
        {
            if (IPortConnectionSource.FindConnectedPortData(m_ConnectedPortData, nodeID, other.ID) == null)
            {
                m_ConnectedPortData.Add(new ConnectedPortData
                {
                    NodeID = nodeID,
                    PortID = other.ID
                });
                ConnectedCount++;
                other.ConnectedCount++;
            }
        }

        public void Disconnect(int nodeID, BasePort other)
        {
            for (int i = 0; i < m_ConnectedPortData.Count; i++)
            {
                if (m_ConnectedPortData[i].NodeID == nodeID && m_ConnectedPortData[i].PortID == other.ID)
                {
                    m_ConnectedPortData.RemoveAt(i);
                    ConnectedCount--;
                    other.ConnectedCount--;
                    return;
                }
            }
        }

        public void CheckConnectValid(IReadOnlyList<BaseNode> nodes)
        {
            if (m_Capacity == Port.Capacity.Single)
            {
                if (ConnectedCount > 1)
                {
                    Debug.LogError($"port id {ID} is single connect,but connected multi count");
                }
            }
            //在这里判断是否存在重复数据
        }

        public ConnectedPortData ConnectedPortDataIterator(int index)
        {
            if (m_ConnectedPortData != null)
            {
                if (index < m_ConnectedPortData.Count)
                {
                    return m_ConnectedPortData[index];
                }
            }
            return null;
        }

        public override string GetConnectTip()
        {
            return "help editor to show other port";
        }
    }
}
#endif