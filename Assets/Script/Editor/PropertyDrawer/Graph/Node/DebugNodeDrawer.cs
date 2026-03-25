using System.Collections.Generic;
using UnityEngine.UIElements;
using YBFramework.Component;
using YBFramework.MyEditor.Common;

namespace YBFramework.MyEditor
{
    [Drawer(typeof(DebugNode))]
    public class DebugNodeDrawer : INodeExtensionContainerDrawer
    {
        private DebugNode m_Node;

        private VisualElement m_PortContainer;

        private NodeView m_NodeView;

        public VisualElement DrawNodeView(NodeView nodeView, BaseNode node)
        {
            m_NodeView = nodeView;
            m_Node = (node as DebugNode)!;
            VisualElement container = new();
            m_PortContainer = new VisualElement();
            Label label = new(m_Node.LogListName);
            container.Add(label);
            container.Add(m_PortContainer);
            VisualElement buttonContainer = new()
            {
                style =
                {
                    flexDirection = FlexDirection.RowReverse
                }
            };
            Button removeButton = new(OnClickRemove)
            {
                text = "-"
            };
            Button addButton = new(OnClickAdd)
            {
                text = "+"
            };
            buttonContainer.Add(removeButton);
            buttonContainer.Add(addButton);
            container.Add(buttonContainer);
            IReadOnlyList<BasePort> logContextInput = m_Node.m_LogContextInput;
            if (logContextInput != null)
            {
                for (int i = 0; i < logContextInput.Count; i++)
                {
                    BasePort port = logContextInput[i];
                    if (DrawerManager.Allocate(port.GetType()) is CommonPortDrawer portDrawer)
                    {
                        VisualElement element = portDrawer.DrawPortView(nodeView, port);
                        element.CommonBorder();
                        m_PortContainer.Add(element);
                    }
                }
            }
            m_NodeView.inputContainer.Add(container);
            return m_PortContainer;
        }

        private void OnClickAdd()
        {
            FuncPort<int> port = new();
            m_Node.Add(port);
            port.ID = m_NodeView.NodeAsset.AllocateID();
            CommonPortDrawer portDrawer = (CommonPortDrawer)DrawerManager.Allocate(typeof(FuncPort<int>));
            VisualElement view = portDrawer.DrawPortView(m_NodeView, port);
            m_PortContainer.Add(view);
            m_NodeView.NodeAsset.SetSelfDirty();
        }

        private void OnClickRemove()
        {
            //不能使用交换位置移除
        }
    }
}