using System;
using System.Reflection;
using UnityEngine;
#if UNITY_EDITOR
using YBFramework.MyEditor;
using UnityEditor.Experimental.GraphView;
#endif

namespace YBFramework.Component
{
    [Serializable]
    [NodeMenu("Test/Example", GraphType.Test1)]
    public sealed class ExampleNode : BaseNode
    {
        private static readonly MethodInfo s_MethodInfo = typeof(ExampleNode).GetMethod(nameof(Add), BindingFlags.Instance | BindingFlags.NonPublic);

        private static readonly MethodInfo s_MethodInfo1 = typeof(ExampleNode).GetMethod(nameof(GetInt), BindingFlags.Instance | BindingFlags.NonPublic);

        [SerializeField] private FuncPort<string> m_StringInput = new();

        [SerializeField] private MethodPort m_LogicInput = new(s_MethodInfo);

        [SerializeField] private MethodPort m_ValueOutput = new(s_MethodInfo1);

        [SerializeField] private ActionPort m_LogicOutput = new();

        private void Add()
        {
            m_LogicOutput.Invoke();
        }

        private int GetInt()
        {
            return 0;
        }

        protected override BasePort PortIterator(int index)
        {
            switch (index)
            {
                case 0:
                    return m_StringInput;
                case 1:
                    return m_LogicInput;
                case 2:
                    return m_LogicOutput;
                case 3:
                    return m_ValueOutput;
                default:
                    return null;
            }
        }

        public override BaseNode Clone()
        {
            ExampleNode node = new();
            CopyPort(this, node);
            return node;
        }
#if UNITY_EDITOR
        public override void InitNodeInfo()
        {
            m_StringInput.PortViewInfo = new PortViewInfo("字符串输入", Direction.Input, Port.Capacity.Single, Color.blue);
            m_LogicInput.PortViewInfo = new PortViewInfo("逻辑输入", Direction.Input, Port.Capacity.Multi, Color.red);
            m_LogicOutput.PortViewInfo = new PortViewInfo("逻辑输出", Direction.Output, Port.Capacity.Multi, Color.red);
            m_ValueOutput.PortViewInfo = new PortViewInfo("int值输出", Direction.Output, Port.Capacity.Multi, Color.blue);
        }
#endif
    }
}