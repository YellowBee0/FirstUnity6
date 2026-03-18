/*using System;
using System.Collections.Generic;
using UnityEngine.UIElements;
using YBFramework.MyEditor.Common;

namespace YBFramework.MyEditor
{
    //TODO:改变类型后连线也需要断开，连线后值就不能再修改
    [Drawer(typeof(DynamicValueFuncPort<>))]
    public sealed class DynamicValueFuncPortDrawer<TValue> : CommonPortDrawer
    {
        private DynamicValueFuncPort<TValue> m_Port;

        private VisualElement m_Container;

        private VisualElement m_BindableElement;

        private NodeView m_NodeView;

        public override VisualElement DrawPortView(NodeView nodeView, PortDrawTarget target)
        {
            m_NodeView = nodeView;
            m_Port = target as DynamicValueFuncPort<TValue>;
            if (m_Port != null)
            {
                DynamicValuePortViewInfo portViewInfo = m_Port.GetDynamicValuePortViewInfo();
                VisualElement portView = base.DrawPortView(nodeView, target);
                m_Container = new VisualElement();
                string selectedTypeName = m_Port.GetSelectedTypeName();
                IReadOnlyList<Type> optionalTypes = portViewInfo.GetOptionalTypes();
                List<string> displayTypeNames = new List<string>(optionalTypes.Count);
                for (int i = 0; i < optionalTypes.Count; i++)
                {
                    displayTypeNames.Add(optionalTypes[i].Name);
                }
                PopupField<string> typeSelectField = new("请选择类型", displayTypeNames, selectedTypeName);
                typeSelectField.RegisterValueChangedCallback(OnSelectionChange);
                m_Container.Add(portView);
                m_Container.Add(typeSelectField);
                m_BindableElement = DrawField(selectedTypeName);
                if (m_BindableElement != null)
                {
                    m_Container.Add(m_BindableElement);
                }
                return m_Container;
            }
            return null;
        }

        private VisualElement DrawField(string typeName)
        {
            Type drawType = m_Port.GetDynamicValuePortViewInfo().GetType(typeName);
            return drawType != null ? ValuePortDrawHelper.DrawValueField(m_Port, m_NodeView.NodeAsset, drawType) : null;
        }

        private void OnSelectionChange(ChangeEvent<string> evt)
        {
            string selectedTypeName = evt.newValue;
            m_Port.SetSelectedTypeName(selectedTypeName);
            m_Port.SetValue(null);
            m_BindableElement?.RemoveFromHierarchy();
            m_BindableElement = DrawField(selectedTypeName);
            if (m_BindableElement != null)
            {
                m_Container.Add(m_BindableElement);
            }
            m_NodeView.NodeAsset.SetSelfDirty();
        }
    }
}*/

