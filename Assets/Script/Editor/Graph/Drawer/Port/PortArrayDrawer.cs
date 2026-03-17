/*using System.Collections.Generic;
using UnityEngine.UIElements;
using YBFramework.Component;
using YBFramework.MyEditor.Common;

namespace YBFramework.MyEditor
{
    [Drawer(typeof(PortArray<>))]
    public sealed class PortArrayDrawer<TPort> : CommonPortDrawer where TPort : BasePort, new()
    {
        private NodeView m_NodeView;

        private VisualElement m_PortContainer;

        private PortArray<TPort> m_PortArray;

        private int m_Counter;

        public override VisualElement DrawPortView(NodeView nodeView, PortDrawTarget target)
        {
            m_NodeView = nodeView;
            m_PortArray = (PortArray<TPort>)target;
            PortArrayPortViewInfo portViewInfo = m_PortArray.GetPortArrayPortViewInfo();
            VisualElement container = new();

            Label title = new(portViewInfo.Name);
            container.Add(title);

            m_PortContainer = new VisualElement();
            IReadOnlyList<TPort> ports = m_PortArray.GetPorts();
            if (ports != null)
            {
                for (int i = 0; i < ports.Count; i++)
                {
                    ports[i].SetPortViewInfo(portViewInfo.PortElementPortViewInfo);
                    CommonPortDrawer portDrawer = (CommonPortDrawer)DrawerManager.Allocate(typeof(TPort));
                    VisualElement view = portDrawer.DrawPortView(nodeView, ports[i]);
                    m_PortContainer.Add(view);
                }
            }
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
            return container;
        }

        private void OnClickAdd()
        {
            TPort port = new();
            m_PortArray.Add(port);
            port.SetID(m_NodeView.NodeAsset.AllocateID());
            port.SetPortViewInfo(m_PortArray.GetPortArrayPortViewInfo().PortElementPortViewInfo);
            CommonPortDrawer portDrawer = (CommonPortDrawer)DrawerManager.Allocate(typeof(TPort));
            VisualElement view = portDrawer.DrawPortView(m_NodeView, port);
            m_PortContainer.Add(view);
            m_NodeView.NodeAsset.SetSelfDirty();
        }

        private void OnClickRemove()
        {
            //不能使用交换位置移除
        }
    }
}*/