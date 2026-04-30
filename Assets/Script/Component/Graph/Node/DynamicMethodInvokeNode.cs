using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using YBFramework.MyEditor;

namespace YBFramework.Component
{
    [Serializable]
#if UNITY_EDITOR
    [NodeMenu("动态函数调用节点", GraphType.Everything)]
#endif
    public sealed class DynamicMethodInvokeNode : BaseNode
    {
        private static readonly MethodInfo s_InvokeMethod;

        private static readonly MethodInfo s_GetReturnValueMethod;

        [SerializeField] private MethodPort m_InvokeMethodPort = new();

        [SerializeField] private ActionPort m_LogicOutput = new();

        [SerializeField] private MethodPort m_ReturnValuePort = new();

        [SerializeField] private FuncPort<MethodInfo> m_GetMethodInfoPort = new();

        [SerializeField] private FuncPort<object> m_GetTargetPort = new();

        [SerializeField] private List<FuncPort<object>> m_ParameterPorts;

        private object[] parameters;

        private object m_ReturnValue;

        static DynamicMethodInvokeNode()
        {
            Type thisType = typeof(DynamicMethodInvokeNode);
            s_InvokeMethod = thisType.GetMethod(nameof(InvokeMethod));
            s_GetReturnValueMethod = thisType.GetMethod(nameof(GetReturnValue));
        }

        private void InvokeMethod()
        {
            MethodInfo methodInfo = m_GetMethodInfoPort.Invoke();
            if (methodInfo != null)
            {
                for (int i = 0; i < m_ParameterPorts.Count; i++)
                {
                    parameters[i] = m_ParameterPorts[i].Invoke();
                }
                object target = m_GetTargetPort.Invoke();
                m_ReturnValue = methodInfo.Invoke(target, parameters);
            }
            else
            {
                m_ReturnValue = null;
            }
            m_LogicOutput.Invoke();
        }

        private object GetReturnValue()
        {
            return m_ReturnValue;
        }

        protected override BasePort PortIterator(int index)
        {
            switch (index)
            {
                case 0:
                    return m_InvokeMethodPort;
                case 1:
                    return m_LogicOutput;
                case 2:
                    return m_ReturnValuePort;
                case 3:
                    return m_GetMethodInfoPort;
                case 4:
                    return m_GetTargetPort;
                default:
                    if (m_ParameterPorts != null)
                    {
                        int listIndex = index - 5;
                        return listIndex >= 0 && listIndex < m_ParameterPorts.Count ? m_ParameterPorts[listIndex] : null;
                    }
                    return null;
            }
        }

        public override BaseNode Clone()
        {
            DynamicMethodInvokeNode node = new();
            int count = m_ParameterPorts.Count;
            m_ParameterPorts = new List<FuncPort<object>>(count);
            parameters = new object[count];
            CopyPort(this, node);
            return node;
        }

        public override void InitPortInfo()
        {
            m_InvokeMethodPort.SetMethodInfo(s_InvokeMethod);
            m_ReturnValuePort.SetMethodInfo(s_GetReturnValueMethod);
        }
#if UNITY_EDITOR
        protected override BasePort PortDrawTargetIterator(int index)
        {
            switch (index)
            {
                case 0:
                    return m_InvokeMethodPort;
                case 1:
                    return m_LogicOutput;
                case 2:
                    return m_ReturnValuePort;
                case 3:
                    return m_GetMethodInfoPort;
                case 4:
                    return m_GetTargetPort;
                default:
                    return null;
            }
        }

        public override void InitNodeViewInfo()
        {
            base.InitNodeViewInfo();
            m_InvokeMethodPort.InitPortViewInfo(nameof(m_InvokeMethodPort), "调用动态函数", Direction.Input, Port.Capacity.Multi, Color.red);
            m_LogicOutput.InitPortViewInfo(nameof(m_LogicOutput), "调用结束", Direction.Input, Port.Capacity.Multi, Color.red);
            m_ReturnValuePort.InitPortViewInfo(nameof(m_ReturnValuePort), "返回值", Direction.Output, Port.Capacity.Multi, Color.blue);
            m_GetMethodInfoPort.InitPortViewInfo(nameof(m_GetMethodInfoPort), "调用的函数", Direction.Input, Port.Capacity.Single, Color.blue);
            m_GetTargetPort.InitPortViewInfo(nameof(m_GetTargetPort), "调用实例", Direction.Input, Port.Capacity.Single, Color.blue);
            if (m_ParameterPorts != null)
            {
                for (int i = 0; i < m_ParameterPorts.Count; i++)
                {
                    m_ParameterPorts[i].InitPortViewInfo($"array.data[{i}]", "参数", Direction.Input, Port.Capacity.Single, Color.blue);
                } 
            }
        }
#endif
    }
}