using System;
using System.Collections.Generic;
using UnityEngine.UIElements;
using YBFramework.Component;
using YBFramework.MyEditor.Common;

namespace YBFramework.MyEditor
{
    [Drawer(typeof(DebugNode))]
    public class DebugNodeDrawer : CommonNodeDrawer
    {
        private sealed class SelectablePort
        {
            private readonly VisualElement m_Container;

            private VisualElement m_PortView;

            public int Index;

            public SelectablePort(VisualElement container)
            {
                m_Container = container;
            }
        }

        private readonly List<string> m_TypeNames = new();

        private DebugNode m_Node;

        private VisualElement m_PortContainer;

        private NodeView m_NodeView;

        public override void DrawNodeView(NodeView nodeView, BaseNode node)
        {
            base.DrawNodeView(nodeView, node);
            m_NodeView = nodeView;
            m_Node = (node as DebugNode)!;
            for (int i = 0; i < m_Node.OptionalTypes.Count; i++)
            {
                m_TypeNames.Add(m_Node.OptionalTypes[i].Name);
            }
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
        }

        private void OnClickAdd()
        {
            VisualElement container = new();
            ValueTuple<List<string>, List<Type>> group = DerivedTypeManager.GetDerivedTypes(typeof(object));
            DerivedTypePopupField derivedTypePopupField = new("select type", group.Item1, group.Item2);
            if (type != null)
            {
                Type portType = typeof(FuncPort<>).MakeGenericType(type);
                if (Activator.CreateInstance(portType) is DelegatePort port)
                {
                    m_Node.m_LogContextInput.Add(port);
                    port.ID = m_NodeView.NodeAsset.AllocateID();
                    CommonPortDrawer portDrawer = (CommonPortDrawer)DrawerManager.Allocate(portType);
                    VisualElement view = portDrawer.DrawPortView(m_NodeView, port);
                    PopupField<string> popupField = new("选择类型", m_TypeNames, 0);
                    VisualElement visualElement = new();
                    visualElement.Add(popupField);
                    visualElement.Add(view);
                    new SelectablePort(this, visualElement, popupField, view, port.ID);
                    m_PortContainer.Add(visualElement);
                    m_NodeView.NodeAsset.SetSelfDirty();
                }
            }
        }

        private void OnClickRemove()
        {
            //不能使用交换位置移除
        }
    }
}