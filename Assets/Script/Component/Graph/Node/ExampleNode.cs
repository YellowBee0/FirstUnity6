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
        private static readonly MethodInfo s_AddMethod;

        private static readonly MethodInfo s_GetIntMethod;

        [SerializeField] private FuncPort<string> m_StringInput = new();

        [SerializeField] private MethodPort m_LogicInput = new();

        [SerializeField] private MethodPort m_ValueOutput = new();

        [SerializeField] private ActionPort m_LogicOutput = new();

        static ExampleNode()
        {
            Type thisType = typeof(ExampleNode);
            s_AddMethod = thisType.GetMethod(nameof(Add), BindingFlags.Instance | BindingFlags.NonPublic);
            s_GetIntMethod = thisType.GetMethod(nameof(GetInt), BindingFlags.Instance | BindingFlags.NonPublic);
        }

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

        public override void InitPortInfo()
        {
            m_LogicInput.SetMethodInfo(s_AddMethod);
            m_ValueOutput.SetMethodInfo(s_GetIntMethod);
        }
#if UNITY_EDITOR
        public override void InitNodeViewInfo()
        {
            InitPortInfo();
            m_StringInput.InitPortViewInfo(nameof(m_StringInput), "字符串输入", Direction.Input, Port.Capacity.Single, Color.blue);
            m_LogicInput.InitPortViewInfo(nameof(m_LogicInput), "逻辑输入", Direction.Input, Port.Capacity.Multi, Color.red);
            m_LogicOutput.InitPortViewInfo(nameof(m_LogicOutput), "逻辑输出", Direction.Output, Port.Capacity.Multi, Color.red);
            m_ValueOutput.InitPortViewInfo(nameof(m_ValueOutput), "int值输出", Direction.Output, Port.Capacity.Multi, Color.blue);
        }
#endif
    }
}