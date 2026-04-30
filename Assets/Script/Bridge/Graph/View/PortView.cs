#if UNITY_EDITOR
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
                    NodeAsset nodeAsset = null;
                    //TODO：这里的连接需要重做，在FuncPort中在Connect的时候会出现不能连接的情况，可能需要在CanConnect中加上Node的ID
                    if (input.BindPort is IPortConnectionSource inputConnectionSource && inputConnectionSource.CanConnect(output.BindPort))
                    {
                        nodeAsset = output.NodeView.BindNodeAsset;
                        inputConnectionSource.Connect(nodeAsset.GetNodeID(), output.BindPort);
                        isConnected = true;
                    }
                    else if (output.BindPort is IPortConnectionSource outputConnectionSource && outputConnectionSource.CanConnect(input.BindPort))
                    {
                        nodeAsset = input.NodeView.BindNodeAsset;
                        outputConnectionSource.Connect(nodeAsset.GetNodeID(), input.BindPort);
                        isConnected = true;
                    }
                    if (isConnected)
                    {
                        nodeAsset.SaveData();
                        if (input.capacity == Capacity.Single)
                        {
                            foreach (Edge connection in input.connections)
                            {
                                if (connection != edge)
                                {
                                    connection.input.Disconnect(connection);
                                    connection.output.Disconnect(connection);
                                    customGraphView.RemoveElement(connection);
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
                                    connection.input.Disconnect(connection);
                                    connection.output.Disconnect(connection);
                                    customGraphView.RemoveElement(connection);
                                    break;
                                }
                            }
                        }
                        edge.input.Connect(edge);
                        edge.output.Connect(edge);
                        customGraphView.AddElement(edge);
                    }
                }
            }
        }

        public NodeView NodeView;

        public readonly BasePort BindPort;

        public PortView(BasePort bindPort, string name, Direction direction, Capacity capacity, Color color) : base(Orientation.Horizontal, direction, capacity, null)
        {
            BindPort = bindPort;
            tooltip = bindPort.GetConnectTip();
            portName = name;
            portColor = color;
            m_EdgeConnector = new EdgeConnector<Edge>(new EdgeConnectorListener());
            this.AddManipulator(m_EdgeConnector);
        }
    }
}
#endif