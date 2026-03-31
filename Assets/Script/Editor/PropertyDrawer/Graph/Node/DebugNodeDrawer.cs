using System;
using System.Collections.Generic;
using UnityEngine;
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

            public bool IsSelected;

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
                    m_DebugNodeDrawer.m_Node.LogContextInput[Index] = port;
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

        private static readonly ValueTuple<List<string>, List<Type>> s_TypeGroup = DerivedTypeManager.GetDerivedTypes(typeof(object));

        private readonly List<LogContextView> m_SelectedLogContextViews = new();

        private VisualElement m_PortContainer;

        private NodeView m_NodeView;

        private DebugNode m_Node;

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
            if (m_Node.LogContextInput != null)
            {
                for (int i = 0; i < m_Node.LogContextInput.Count; i++)
                {
                    DrawLogContextView(m_Node.LogContextInput[i], i);
                }
            }
            m_NodeView.inputContainer.Add(container);
        }

        private void DrawLogContextView(BasePort port, int index)
        {
            DerivedTypePopupField derivedTypePopupField = new("select type", s_TypeGroup.Item1, s_TypeGroup.Item2);
            port.PortViewInfo = m_Node.ListPortViewInfo;
            Type valueType = port.GetType().GetGenericArguments()[0];
            derivedTypePopupField.Initialize(valueType);
            CommonPortDrawer portDrawer = (CommonPortDrawer)DrawerManager.Allocate(port.GetType());
            LogContextView logContextView = new(this, portDrawer.DrawPortView(m_NodeView, port), derivedTypePopupField)
            {
                Index = index
            };
            logContextView.RegisterCallback<ClickEvent>(OnMouseClick);
            logContextView.CommonBorder();
            m_PortContainer.Add(logContextView);
        }

        private void OnClickAdd()
        {
            DelegatePort port = new FuncPort<object>();
            port.ID = m_NodeView.NodeAsset.AllocateID();
            DrawLogContextView(port, m_Node.LogContextInput.Count);
            m_Node.LogContextInput.Add(port);
            m_NodeView.NodeAsset.SetSelfDirty();
        }

        private void OnClickRemove()
        {
            for (int i = 0; i < m_SelectedLogContextViews.Count; i++)
            {
                LogContextView logContextView = m_SelectedLogContextViews[i];
                logContextView.RemoveFromHierarchy();
                logContextView.UnregisterCallback<ClickEvent>(OnMouseClick);
                m_Node.LogContextInput.RemoveAt(logContextView.Index);
            }
            m_SelectedLogContextViews.Clear();
        }

        private void OnMouseClick(ClickEvent evt)
        {
            if (evt.currentTarget is LogContextView targetView)
            {
                if (targetView.IsSelected)
                {
                    targetView.IsSelected = false;
                    targetView.style.backgroundColor = new Color(0.2f, 0.2f, 0.2f);
                    m_SelectedLogContextViews.Remove(targetView);
                }
                else
                {
                    targetView.IsSelected = true;
                    targetView.style.backgroundColor = new Color(0, 0, 1, 0.2f);
                    m_SelectedLogContextViews.Add(targetView);
                }
            }
        }
    }
}