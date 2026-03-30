using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.UIElements;
using YBFramework.Common;
using YBFramework.Component;
using YBFramework.MyEditor.Common;

namespace YBFramework.MyEditor
{
    [Drawer(typeof(DebugNode))]
    public class DebugNodeDrawer : CommonNodeDrawer
    {
        private sealed class SelectablePort
        {
            private readonly DebugNodeDrawer m_DebugNodeDrawer;

            private readonly VisualElement m_Container;

            private VisualElement m_PortView;

            public int Index;

            public SelectablePort(DebugNodeDrawer debugNodeDrawer, VisualElement container, DerivedTypePopupField popupField, VisualElement portView)
            {
                m_Container = container;
                m_DebugNodeDrawer = debugNodeDrawer;
                m_PortView = portView;
                popupField.RegisterTypeChangedCallBack(OnTypeChanged);
                container.Add(popupField);
                container.Add(portView);
            }

            private void OnTypeChanged(Type newType)
            {
                Type portType = typeof(FuncPort<>).MakeGenericType(newType);
                if (Activator.CreateInstance(portType) is DelegatePort port)
                {
                    m_DebugNodeDrawer.m_Node.m_LogContextInput[Index] = port;
                    port.ID = m_DebugNodeDrawer.m_NodeView.NodeAsset.AllocateID();
                    CommonPortDrawer portDrawer = (CommonPortDrawer)DrawerManager.Allocate(portType);
                    VisualElement portView = portDrawer.DrawPortView(m_DebugNodeDrawer.m_NodeView, port);
                    m_PortView?.RemoveFromHierarchy();
                    m_PortView = portView;
                    m_Container.Add(m_PortView);
                    m_DebugNodeDrawer.m_NodeView.NodeAsset.SetSelfDirty();
                }
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
            SelectablePort selectablePort = new SelectablePort(this, container, derivedTypePopupField, null);
            m_PortContainer.Add(container);
        }

        private void OnClickRemove()
        {
            //不能使用交换位置移除
        }
    }
}