using System;
using System.Collections.Generic;
using UnityEngine;

namespace YBFramework.Component
{
    [Serializable]
    public sealed class TemplateNode : BaseNode
    {
        [SerializeField] private GraphAsset m_GraphAsset;

        private TemplateHelpNode m_TemplateHelpEnter = new();

        private TemplateHelpNode m_TemplateHelpExit = new();

        protected override BasePort PortIterator(int index)
        {
            throw new NotImplementedException();
        }

        public override BaseNode Clone()
        {
            throw new NotImplementedException();
        }

        protected override BasePort PortDrawTargetIterator(int index)
        {
            IPortConnectionSource connectionSource = m_TemplateHelpEnter.GetCommonPort();
            foreach (ConnectedPortData connectedPortData in connectionSource.GetConnectedPortDataEnumerator())
            {
                IReadOnlyList<NodeAsset> nodeAssets = m_GraphAsset.GetNodeAssets();
                for (int i = 0; i < nodeAssets.Count; i++)
                {
                    if (nodeAssets[i].GetNodeID() == connectedPortData.NodeID)
                    {
                        return nodeAssets[i].GetNode().GetPort(connectedPortData.PortID);
                    }
                }
            }
            //输出也这么做
            return null;
        }

        public override void InitNodeInfo()
        {
            throw new NotImplementedException();
        }
    }
}