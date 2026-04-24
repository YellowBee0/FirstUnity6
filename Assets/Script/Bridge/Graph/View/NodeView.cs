using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using YBFramework.Component;

namespace YBFramework.MyEditor
{
    public sealed class NodeView : Node
    {
        private readonly CustomGraphView GraphView;

        private readonly List<PortView> m_PortViews = new();

        public readonly NodeAsset NodeAsset;

        public NodeView(CustomGraphView graphView, NodeAsset nodeAsset)
        {
            GraphView = graphView;
            NodeAsset = nodeAsset;
            title = nodeAsset.name;
            SetPosition(new Rect(nodeAsset.GetPosition(), Vector2.zero));
            BaseNode baseNode = nodeAsset.GetNode();
            baseNode.InitNodeInfo();
            inputContainer.style.display = inputContainer.childCount > 0 ? DisplayStyle.Flex : DisplayStyle.None;
            outputContainer.style.display = outputContainer.childCount > 0 ? DisplayStyle.Flex : DisplayStyle.None;
        }

        public PortView GetPortView(int portID)
        {
            for (int i = 0; i < m_PortViews.Count; i++)
            {
                if (m_PortViews[i].Port.ID == portID)
                {
                    return m_PortViews[i];
                }
            }
            return null;
        }

        public IReadOnlyList<PortView> GetPortViews()
        {
            return m_PortViews;
        }

        public void AddPortView(PortView portView)
        {
            m_PortViews.Add(portView);
            GraphView.AddPortView(portView);
        }

        public void RemovePortView(PortView portView)
        {
            m_PortViews.Remove(portView);
            GraphView.RemovePortView(portView);
        }

        public void SetName(string nodeName)
        {
            title = nodeName;
            NodeAsset.name = nodeName;
        }
    }
}