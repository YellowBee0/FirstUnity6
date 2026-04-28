/*#if UNITY_EDITOR
using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using YBFramework.MyEditor;

namespace YBFramework.Component
{
    [Serializable]
    [NodeMenu("模板出入口", GraphType.Everything)]
    public sealed class TemplateHelpNode : BaseNode
    {
        [SerializeField] private CommonPort m_Port = new();

        public CommonPort GetCommonPort()
        {
            return m_Port;
        }

        protected override BasePort PortIterator(int index)
        {
            return index == 0 ? m_Port : null;
        }

        public override BaseNode Clone()
        {
            return null;
        }

        public override void InitNodeViewInfo()
        {
            m_Port.InitPortViewInfo(nameof(m_Port), "输入端口", Direction.Input, Port.Capacity.Multi, Color.green);
        }
    }
}
#endif*/