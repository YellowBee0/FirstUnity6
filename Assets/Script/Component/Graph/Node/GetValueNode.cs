using System;
using System.Reflection;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using YBFramework.MyEditor;

namespace YBFramework.Component
{
    [Serializable]
    [NodeMenu("Test/获取值", GraphType.Everything)]
    public sealed class GetValueNode : BaseNode
    {
        private static readonly MethodInfo[] s_GetValueMethods =
        {
            typeof(GetValueNode).GetMethod(nameof(GetIntValue), BindingFlags.Instance | BindingFlags.NonPublic),
            typeof(GetValueNode).GetMethod(nameof(GetStringValue), BindingFlags.Instance | BindingFlags.NonPublic)
        };

        [SerializeField] private MethodPort m_IntOutput = new(s_GetValueMethods[0]);

        [SerializeField] private MethodPort m_StringOutput = new(s_GetValueMethods[1]);

        [HideInInspector] [SerializeField] private int m_IntValue;

        [HideInInspector] [SerializeField] private string m_StringValue;

        private int GetIntValue()
        {
            return m_IntValue;
        }

        private string GetStringValue()
        {
            return m_StringValue;
        }

        protected override BasePort PortIterator(int index)
        {
            switch (index)
            {
                case 0:
                    return m_IntOutput;
                case 1:
                    return m_StringOutput;
                default:
                    return null;
            }
        }

        public override BaseNode Clone()
        {
            GetValueNode node = new()
            {
                ID = ID,
                m_IntValue = m_IntValue,
                m_StringValue = m_StringValue
            };
            CopyPort(this, node);
            return node;
        }

        public override void InitNodeInfo()
        {
            m_IntOutput.PortViewInfo = new PortViewInfo("int值输出", Direction.Output, Port.Capacity.Multi, Color.blue);
            m_StringOutput.PortViewInfo = new PortViewInfo("string值输出", Direction.Output, Port.Capacity.Multi, Color.blue);
        }
    }
}