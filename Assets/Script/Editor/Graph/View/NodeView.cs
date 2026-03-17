using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using YBFramework.Component;
using YBFramework.MyEditor.Common;

namespace YBFramework.MyEditor
{
    public sealed class NodeView : Node
    {
        public readonly NodeAsset NodeAsset;

        private readonly CustomGraphView GraphView;

        private readonly List<PortView> m_PortViews = new();

        public NodeView(CustomGraphView graphView, NodeAsset nodeAsset)
        {
            GraphView = graphView;
            NodeAsset = nodeAsset;
            title = nodeAsset.name;
            SetPosition(new Rect(nodeAsset.GetPosition(), Vector2.zero));
            BaseNode baseNode = nodeAsset.GetNode();
            baseNode.InitNodeInfo();
            //TODO:这一部分可能需要写成一个函数
            bool isInputAddElement = false;
            bool isOutputAddElement = false;
            foreach (PortDrawTarget portDrawTarget in baseNode.GetPortDrawTargetEnumerable())
            {
                if (DrawerManager.Allocate(portDrawTarget.GetType()) is CommonPortDrawer portDrawer)
                {
                    VisualElement element = portDrawer.DrawPortView(this, portDrawTarget);
                    element.CommonBorder();
                    if (portDrawTarget.PortViewInfo.Direction == Direction.Input)
                    {
                        inputContainer.Add(element);
                        isInputAddElement = true;
                    }
                    else
                    {
                        outputContainer.Add(element);
                        isOutputAddElement = true;
                    }
                }
            }
            inputContainer.style.display = isInputAddElement ? DisplayStyle.Flex : DisplayStyle.None;
            outputContainer.style.display = isOutputAddElement ? DisplayStyle.Flex : DisplayStyle.None;
            if (DrawerManager.Allocate(baseNode.GetType()) is INodeExtensionContainerDrawer nodeExtensionContainerDrawer)
            {
                nodeExtensionContainerDrawer.DrawNodeView(this, baseNode);
            }
        }

        public PortView GetPortView(int portID)
        {
            for (int i = 0; i < m_PortViews.Count; i++)
            {
                if (m_PortViews[i].Port.GetID() == portID)
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

        public void RemovePortContainerElement(Direction direction, VisualElement element)
        {
            if (direction == Direction.Input)
            {
                inputContainer.Remove(element);
            }
            else
            {
                outputContainer.Remove(element);
            }
        }

        public void SetName(string nodeName)
        {
            title = nodeName;
            NodeAsset.name = nodeName;
        }
    }
}