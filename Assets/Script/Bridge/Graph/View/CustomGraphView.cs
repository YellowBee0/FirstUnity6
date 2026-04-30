#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using YBFramework.Component;

namespace YBFramework.MyEditor
{
    public sealed class CustomGraphView : GraphView
    {
        private static readonly List<NodeView> s_RemovedNodeViewTemp = new();

        public readonly GraphAsset m_BindGraphAsset;

        private readonly List<NodeView> m_NodeViews = new();

        private readonly List<Port> m_InputPortViews = new();

        private readonly List<Port> m_OutputPortViews = new();

        public CustomGraphView(GraphAsset bindGraphAsset, NodeSearchView nodeSearchView)
        {
            m_BindGraphAsset = bindGraphAsset;
            name = bindGraphAsset.name;
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            GridBackground grid = new();
            grid.StretchToParentSize();
            Insert(0, grid);

            this.StretchToParentSize();
            nodeCreationRequest = context => { SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), nodeSearchView); };
            graphViewChanged += OnGraphViewChanged;

            foreach (NodeAsset nodeAsset in bindGraphAsset.NewGetNodeAssets())
            {
                nodeAsset.CreateNodeView(this);
            }

            for (int i = 0; i < m_NodeViews.Count; i++)
            {
                NodeView fromNodeView = m_NodeViews[i];
                foreach (BasePort basePort in fromNodeView.BindNodeAsset.GetNode().GetPortEnumerable())
                {
                    if (basePort is IPortConnectionSource portConnectionSource)
                    {
                        PortView fromPortView = fromNodeView.GetPortView(basePort.ID);
                        foreach (ConnectedPortData connectedPortData in portConnectionSource.GetConnectedPortDataEnumerator())
                        {
                            NodeView toNodeView = GetNodeView(connectedPortData.NodeID);
                            PortView toPortView = toNodeView?.GetPortView(connectedPortData.PortID);
                            if (toPortView != null)
                            {
                                Edge edge = fromPortView.ConnectTo(toPortView);
                                AddElement(edge);
                            }
                        }
                    }
                }
            }
        }

        private NodeView GetNodeView(int nodeID)
        {
            for (int i = 0; i < m_NodeViews.Count; i++)
            {
                if (m_NodeViews[i].BindNodeAsset.GetNodeID() == nodeID)
                {
                    return m_NodeViews[i];
                }
            }
            return null;
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange changeValue)
        {
            if (changeValue.elementsToRemove != null)
            {
                s_RemovedNodeViewTemp.Clear();
                for (int i = 0; i < changeValue.elementsToRemove.Count; i++)
                {
                    if (changeValue.elementsToRemove[i] is NodeView nodeView)
                    {
                        s_RemovedNodeViewTemp.Add(nodeView);
                    }
                    else if (changeValue.elementsToRemove[i] is Edge edge)
                    {
                        PortView input = (PortView)edge.input;
                        PortView output = (PortView)edge.output;
                        if (input.BindPort is IPortConnectionSource inputConnectionSource)
                        {
                            inputConnectionSource.Disconnect(output.NodeView.BindNodeAsset.GetNodeID(), output.BindPort);
                        }
                        else if (output.BindPort is IPortConnectionSource outputConnectionSource)
                        {
                            outputConnectionSource.Disconnect(input.NodeView.BindNodeAsset.GetNodeID(), input.BindPort);
                        }
                    }
                }
                for (int i = 0; i < s_RemovedNodeViewTemp.Count; i++)
                {
                    NodeView nodeViewToRemove = s_RemovedNodeViewTemp[i];
                    m_NodeViews.Remove(nodeViewToRemove);
                    foreach (PortView portView in nodeViewToRemove.GetPortViews())
                    {
                        RemovePortView(portView);
                    }
                    m_BindGraphAsset.RemoveNodeAsset(nodeViewToRemove.BindNodeAsset);
                }
            }
            if (changeValue.movedElements != null)
            {
                for (int i = 0; i < changeValue.movedElements.Count; i++)
                {
                    if (changeValue.movedElements[i] is NodeView nodeView)
                    {
                        nodeView.BindNodeAsset.SetPosition(nodeView.BindNodeAsset.GetPosition() + changeValue.moveDelta);
                    }
                }
            }
            return changeValue;
        }

        public void AddNodeView(NodeView nodesView)
        {
            m_NodeViews.Add(nodesView);
            AddElement(nodesView);
        }

        public void AddPortView(PortView portView)
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

        public void RemovePortView(PortView portView)
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
            return startPort.direction == Direction.Input ? m_OutputPortViews : m_InputPortViews;
        }
    }
}
#endif