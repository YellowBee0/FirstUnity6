#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using YBFramework.Component;

namespace YBFramework.MyEditor
{
    public sealed class NodeView : Node
    {
        public readonly NodeAsset BindNodeAsset;

        private readonly CustomGraphView m_GraphView;

        private readonly List<PortView> m_PortViews = new();

        public NodeView(NodeAsset bindNodeAsset, CustomGraphView graphView)
        {
            BindNodeAsset = bindNodeAsset;
            m_GraphView = graphView;
        }

        public IEnumerable<PortView> GetPortViews()
        {
            return m_PortViews;
        }

        public PortView GetPortView(int portID)
        {
            for (int i = 0; i < m_PortViews.Count; i++)
            {
                if (m_PortViews[i].BindPort.ID == portID)
                {
                    return m_PortViews[i];
                }
            }
            return null;
        }

        public void AddPortView(PortView portView)
        {
            m_PortViews.Add(portView);
            portView.NodeView = this;
            m_GraphView.AddPortView(portView);
        }

        public void RemovePortView(PortView portView)
        {
            if (m_PortViews.Remove(portView))
            {
                portView.NodeView = null;
                m_GraphView.RemovePortView(portView);
            }
        }
    }
}
#endif