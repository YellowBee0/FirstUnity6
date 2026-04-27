#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using YBFramework.Component;

namespace YBFramework.MyEditor
{
    public sealed class NewCustomGraphView : GraphView
    {
        private readonly GraphAsset m_BindGraphAsset;
        
        private readonly List<NewNodeView> m_NodeViews = new();
        
        private readonly List<Port> m_InputPortViews;
        
        private readonly List<Port> m_OutputPortViews;

        public NewCustomGraphView(GraphAsset bindGraphAsset)
        {
            m_BindGraphAsset = bindGraphAsset;
            foreach (NodeAsset nodeAsset in bindGraphAsset.NewGetNodeAssets())
            {
                nodeAsset.CreateNodeView(this);
            }
        }

        public void AddPortView(NewPortView portView)
        {
            if (portView.direction == Direction.Input)
            {
                m_InputPortViews.Add(portView);
            }
            else
            {
                m_OutputPortViews.Add(portView);
            }
        }
        
        public void RemovePortView(NewPortView portView)
        {
            if (portView.direction == Direction.Input)
            {
                m_InputPortViews.Remove(portView);
            }
            else
            {
                m_OutputPortViews.Remove(portView);
            }
        }
        
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            return startPort.direction == Direction.Input ? m_InputPortViews : m_OutputPortViews;
        }
    }
}
#endif