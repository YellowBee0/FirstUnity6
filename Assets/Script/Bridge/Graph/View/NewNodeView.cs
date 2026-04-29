#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using YBFramework.Component;

namespace YBFramework.MyEditor
{
    public sealed class NewNodeView : Node
    {
        public readonly NodeAsset BindNodeAsset;

        private readonly NewCustomGraphView m_GraphView;

        private readonly List<NewPortView> m_PortViews = new();

        public NewNodeView(NodeAsset bindNodeAsset, NewCustomGraphView graphView)
        {
            BindNodeAsset = bindNodeAsset;
            m_GraphView = graphView;
        }

        public IEnumerable<NewPortView> GetPortViews()
        {
            return m_PortViews;
        }

        public void AddPortView(NewPortView portView)
        {
            m_PortViews.Add(portView);
            portView.NodeView = this;
            m_GraphView.AddPortView(portView);
        }

        public void RemovePortView(NewPortView portView)
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