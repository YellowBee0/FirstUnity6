#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using YBFramework.MyEditor;

namespace YBFramework.Component
{
    [Serializable]
    [NodeMenu("编辑器/打印日志", GraphType.Everything)]
    public sealed class DebugNode : BaseNode
    {
        private static readonly MethodInfo s_LogMethodInfo = typeof(DebugNode).GetMethod(nameof(Log), BindingFlags.Instance | BindingFlags.Public);

        public PortViewInfo ListPortViewInfo;

        [SerializeField] private MethodPort m_LogicInput = new(s_LogMethodInfo);

        [SerializeReference] public List<DelegatePort> LogContextInput = new();

        [NonSerialized] public string LogListName;

        public void Log()
        {
            for (int i = 0; i < LogContextInput.Count; i++)
            {
                Debug.Log(LogContextInput[i].DynamicInvoke());
            }
        }

        protected override BasePort PortIterator(int index)
        {
            if (index == 0)
            {
                return m_LogicInput;
            }
            if (LogContextInput != null)
            {
                int listIndex = index - 1;
                if (listIndex < LogContextInput.Count)
                {
                    return LogContextInput[listIndex];
                }
            }
            return null;
        }

        public override BaseNode Clone()
        {
            DebugNode node = new()
            {
                ID = ID
            };
            //复制所有集合中的元素
            //node.m_LogContextInput.CopySize(m_LogContextInput);
            CopyPort(this, node);
            return node;
        }

        protected override BasePort PortDrawTargetIterator(int index)
        {
            return index == 0 ? m_LogicInput : null;
        }

        public override void InitNodeViewInfo()
        {
            m_LogicInput.InitPortViewInfo("输入", nameof(m_LogicInput), Direction.Input, Port.Capacity.Multi, Color.red);
            LogListName = "打印内容";
            ListPortViewInfo = new PortViewInfo("日志", Direction.Input, Port.Capacity.Single, Color.blue);
        }
    }
}
#endif