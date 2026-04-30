using System;
using System.Reflection;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using YBFramework.MyEditor;

namespace YBFramework.Component
{
    [Serializable]
#if UNITY_EDITOR
    [NodeMenu("动态函数节点", GraphType.Everything)]
#endif
    public sealed class DynamicMethodNode : BaseNode
    {
        private static readonly MethodInfo s_GetMethodInfoMethod = typeof(DynamicMethodNode).GetMethod(nameof(GetMethodInfo));

        [SerializeField] private MethodPort m_BindMethodPort = new();

        [SerializeField] private string m_BindTypeName;

        [SerializeField] private string m_BindMethodName;

        private MethodInfo m_MethodInfo;

        private MethodInfo GetMethodInfo()
        {
            return m_MethodInfo;
        }

        protected override BasePort PortIterator(int index)
        {
            switch (index)
            {
                case 0:
                    return m_BindMethodPort;
                default:
                    return null;
            }
        }

        public override BaseNode Clone()
        {
            DynamicMethodNode node = new()
            {
                m_MethodInfo = m_MethodInfo
            };
            CopyPort(this, node);
            return node;
        }

        public override void InitPortInfo()
        {
            if (!string.IsNullOrEmpty(m_BindTypeName))
            {
                Type selectedType = Type.GetType(m_BindTypeName);
                if (selectedType != null)
                {
                    if (!string.IsNullOrEmpty(m_BindMethodName))
                    {
                        m_MethodInfo = selectedType.GetMethod(m_BindMethodName);
                    }
                }
            }
            m_BindMethodPort.SetMethodInfo(s_GetMethodInfoMethod);
        }

#if UNITY_EDITOR

        public override void InitNodeViewInfo()
        {
            base.InitNodeViewInfo();
            m_BindMethodPort.InitPortViewInfo(nameof(m_BindMethodPort), "绑定的函数", Direction.Output, Port.Capacity.Multi, Color.blue);
        }

#endif
    }
}