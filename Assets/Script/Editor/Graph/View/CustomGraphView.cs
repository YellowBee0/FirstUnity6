using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using YBFramework.Component;

namespace YBFramework.MyEditor
{
    public sealed class CustomGraphView : GraphView
    {
        private static readonly List<NodeView> s_RemovedNodeViewTemp = new();

        public readonly GraphAsset GraphAsset;

        private readonly List<Port> m_InputPorts = new();

        private readonly List<NodeView> m_NodeViews = new();

        private readonly List<Port> m_OutputPorts = new();

        public CustomGraphView(GraphAsset graphAsset, NodeSearchView nodeSearchView)
        {
            GraphAsset = graphAsset;
            name = graphAsset.name;
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            IReadOnlyList<NodeAsset> nodeAssets = graphAsset.GetNodeAssets();
            foreach (NodeAsset nodeAsset in nodeAssets)
            {
                NodeView nodeView = new(this, nodeAsset);
                m_NodeViews.Add(nodeView);
                AddElement(nodeView);
            }
            //恢复连线
            for (int i = 0; i < m_NodeViews.Count; i++)
            {
                NodeView fromNodeView = m_NodeViews[i];
                foreach (BasePort basePort in fromNodeView.NodeAsset.GetNode().GetPortEnumerable())
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

            //TODO: 把GridBackground分出来，每次切换GraphView的时候就把这个对象insert进切换的对象
            GridBackground grid = new();
            grid.StretchToParentSize();
            Insert(0, grid);

            this.StretchToParentSize();
            nodeCreationRequest = context => { SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), nodeSearchView); };
            graphViewChanged += OnGraphViewChanged;
        }

        private NodeView GetNodeView(int nodeID)
        {
            for (int i = 0; i < m_NodeViews.Count; i++)
            {
                if (m_NodeViews[i].NodeAsset.GetNodeID() == nodeID)
                {
                    return m_NodeViews[i];
                }
            }
            return null;
        }

        public IReadOnlyList<NodeView> GetNodeViews()
        {
            return m_NodeViews;
        }

        public void AddNodeView(NodeView nodeView)
        {
            m_NodeViews.Add(nodeView);
            AddElement(nodeView);
            GraphAsset.AddNodeAsset(nodeView.NodeAsset);
        }

        public void AddPortView(PortView port)
        {
            if (port.direction == Direction.Input)
            {
                m_InputPorts.Add(port);
            }
            else
            {
                m_OutputPorts.Add(port);
            }
        }

        public void RemovePortView(PortView port)
        {
            if (port.direction == Direction.Input)
            {
                m_InputPorts.Remove(port);
            }
            else
            {
                m_OutputPorts.Remove(port);
            }
        }

        public void AddEdge(Edge edge)
        {
            AddElement(edge);
            edge.input.Connect(edge);
            edge.output.Connect(edge);
        }

        public void RemoveEdge(Edge edge)
        {
            edge.input.Disconnect(edge);
            edge.output.Disconnect(edge);
            RemoveElement(edge);
        }

        public void SetName(string graphName)
        {
            if (string.IsNullOrEmpty(graphName))
            {
                return;
            }
            name = graphName;
            GraphAsset.name = graphName;
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
                        if (input.Port is IPortConnectionSource inputConnectionSource)
                        {
                            inputConnectionSource.Disconnect(output.NodeView.NodeAsset.GetNodeID(), output.Port);
                        }
                        else if (output.Port is IPortConnectionSource outputConnectionSource)
                        {
                            outputConnectionSource.Disconnect(input.NodeView.NodeAsset.GetNodeID(), input.Port);
                        }
                    }
                }
                for (int i = 0; i < s_RemovedNodeViewTemp.Count; i++)
                {
                    NodeView nodeViewToRemove = s_RemovedNodeViewTemp[i];
                    m_NodeViews.Remove(nodeViewToRemove);
                    IReadOnlyList<PortView> portViews = nodeViewToRemove.GetPortViews();
                    for (int j = 0; j < portViews.Count; j++)
                    {
                        RemovePortView(portViews[j]);
                    }
                    GraphAsset.RemoveNodeAsset(nodeViewToRemove.NodeAsset);
                }
            }
            if (changeValue.movedElements != null)
            {
                for (int i = 0; i < changeValue.movedElements.Count; i++)
                {
                    if (changeValue.movedElements[i] is NodeView nodeView)
                    {
                        nodeView.NodeAsset.SetPosition(nodeView.NodeAsset.GetPosition() + changeValue.moveDelta);
                    }
                }
            }
            return changeValue;
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            return startPort.direction == Direction.Input ? m_OutputPorts : m_InputPorts;
        }
    }
}