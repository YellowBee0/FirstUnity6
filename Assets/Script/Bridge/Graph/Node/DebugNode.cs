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

        private PortViewInfo m_ListPortViewInfo;

        [SerializeField] private MethodPort m_LogicInput = new(s_LogMethodInfo);

        public List<Type> m_OptionalTypes = new();

        public List<string> m_SelectedTypes = new();

        [SerializeReference] public List<DelegatePort> m_LogContextInput;

        [NonSerialized] public string LogListName;

        public void Add(DelegatePort port)
        {
            m_LogContextInput ??= new List<DelegatePort>();
            port.PortViewInfo = m_ListPortViewInfo;
            m_LogContextInput.Add(port);
        }

        public void Remove(DelegatePort port)
        {
            m_LogContextInput.Remove(port);
        }

        public void Log()
        {
            for (int i = 0; i < m_LogContextInput.Count; i++)
            {
                Debug.Log(m_LogContextInput[i].DynamicInvoke());
            }
        }

        protected override BasePort PortIterator(int index)
        {
            if (index == 0)
            {
                return m_LogicInput;
            }
            if (m_LogContextInput != null)
            {
                int listIndex = index - 1;
                if (listIndex < m_LogContextInput.Count)
                {
                    return m_LogContextInput[listIndex];
                }
            }
            return null;
        }

        public override BaseNode Clone()
        {
            DebugNode node = new DebugNode
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

        public override void InitNodeInfo()
        {
            m_LogicInput.PortViewInfo = new PortViewInfo("输入", Direction.Input, Port.Capacity.Multi, Color.red);
            LogListName = "打印内容";
            m_ListPortViewInfo = new PortViewInfo("日志", Direction.Input, Port.Capacity.Single, Color.blue);
            if (m_LogContextInput != null)
            {
                for (int i = 0; i < m_LogContextInput.Count; i++)
                {
                    m_LogContextInput[i].PortViewInfo = m_ListPortViewInfo;
                }
            }
        }
    }
}
#endif