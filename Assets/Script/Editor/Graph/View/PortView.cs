using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using YBFramework.Component;

namespace YBFramework.MyEditor
{
    public sealed class PortView : Port
    {
        private sealed class EdgeConnectorListener : IEdgeConnectorListener
        {
            public void OnDropOutsidePort(Edge edge, Vector2 position)
            {
            }

            public void OnDrop(GraphView graphView, Edge edge)
            {
                if (graphView is CustomGraphView customGraphView && edge.input is PortView input && edge.output is PortView output)
                {
                    bool isConnected = false;
                    if (input.Port is IPortConnectionSource inputConnectionSource && inputConnectionSource.CanConnect(output.Port))
                    {
                        inputConnectionSource.Connect(output.NodeView.NodeAsset.GetNodeID(), output.Port);
                        input.NodeView.NodeAsset.SetSelfDirty();
                        isConnected = true;
                    }
                    else if (output.Port is IPortConnectionSource outputConnectionSource && outputConnectionSource.CanConnect(input.Port))
                    {
                        outputConnectionSource.Connect(input.NodeView.NodeAsset.GetNodeID(), input.Port);
                        output.NodeView.NodeAsset.SetSelfDirty();
                        isConnected = true;
                    }
                    if (isConnected)
                    {
                        if (input.capacity == Capacity.Single)
                        {
                            foreach (Edge connection in input.connections)
                            {
                                if (connection != edge)
                                {
                                    customGraphView.RemoveEdge(connection);
                                    break;
                                }
                            }
                        }
                        if (output.capacity == Capacity.Single)
                        {
                            foreach (Edge connection in output.connections)
                            {
                                if (connection != edge)
                                {
                                    customGraphView.RemoveEdge(connection);
                                    break;
                                }
                            }
                        }
                        customGraphView.AddEdge(edge);
                    }
                }
            }
        }

        public readonly NodeView NodeView;

        public readonly BasePort Port;

        public PortView(NodeView nodeView, BasePort port, string name, Direction direction, Capacity capacity, Color color) : base(Orientation.Horizontal, direction, capacity, null)
        {
            NodeView = nodeView;
            Port = port;
            tooltip = port.GetConnectTip();
            portName = name;
            portColor = color;
            m_EdgeConnector = new EdgeConnector<Edge>(new EdgeConnectorListener());
            this.AddManipulator(m_EdgeConnector);
        }
    }
}