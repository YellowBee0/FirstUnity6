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
        private sealed class LogContextView : VisualElement
        {
            private readonly DebugNodeDrawer m_DebugNodeDrawer;

            private VisualElement m_PortView;

            public int Index;

            public LogContextView(DebugNodeDrawer debugNodeDrawer, VisualElement portView, DerivedTypePopupField popupField)
            {
                m_DebugNodeDrawer = debugNodeDrawer;
                m_PortView = portView;
                popupField.RegisterTypeChangedCallBack(OnTypeChanged);
                Add(popupField);
                Add(portView);
            }

            private void OnTypeChanged(Type newType)
            {
                Type portType = typeof(FuncPort<>).MakeGenericType(newType);
                if (Activator.CreateInstance(portType) is DelegatePort port)
                {
                    m_DebugNodeDrawer.m_Node.m_LogContextInput[Index] = port;
                    port.ID = m_DebugNodeDrawer.m_NodeView.NodeAsset.AllocateID();
                    port.PortViewInfo = m_DebugNodeDrawer.m_Node.ListPortViewInfo;
                    CommonPortDrawer portDrawer = (CommonPortDrawer)DrawerManager.Allocate(portType);
                    VisualElement portView = portDrawer.DrawPortView(m_DebugNodeDrawer.m_NodeView, port);
                    m_PortView?.RemoveFromHierarchy();
                    m_PortView = portView;
                    Add(m_PortView);
                    m_DebugNodeDrawer.m_NodeView.NodeAsset.SetSelfDirty();
                }
            }
        }

        private DebugNode m_Node;

        private VisualElement m_PortContainer;

        private NodeView m_NodeView;

        public override void DrawNodeView(NodeView nodeView, BaseNode node)
        {
            base.DrawNodeView(nodeView, node);
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
        }

        private void OnClickAdd()
        {
            ValueTuple<List<string>, List<Type>> group = DerivedTypeManager.GetDerivedTypes(typeof(object));
            DerivedTypePopupField derivedTypePopupField = new("select type", group.Item1, group.Item2);
            Type defaultType = typeof(object);
            derivedTypePopupField.Initialize(defaultType);
            FuncPort<object> defaultPort = new()
            {
                ID = m_NodeView.NodeAsset.AllocateID(),
                PortViewInfo = m_Node.ListPortViewInfo
            };
            CommonPortDrawer portDrawer = (CommonPortDrawer)DrawerManager.Allocate(defaultPort.GetType());
            LogContextView logContextView = new(this, portDrawer.DrawPortView(m_NodeView, defaultPort), derivedTypePopupField)
            {
                Index = m_Node.m_LogContextInput.Count
            };
            m_Node.m_LogContextInput.Add(defaultPort);
            m_PortContainer.Add(logContextView);
        }

        private void OnClickRemove()
        {
            //不能使用交换位置移除
        }
    }
}