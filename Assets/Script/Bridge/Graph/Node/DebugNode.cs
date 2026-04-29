#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using YBFramework.MyEditor;

namespace YBFramework.Component
{
    [Serializable]
    [NodeMenu("编辑器/打印日志", GraphType.Everything)]
    public sealed class DebugNode : BaseNode
    {
        private static readonly MethodInfo s_LogMethodInfo = typeof(DebugNode).GetMethod(nameof(Log), BindingFlags.Instance | BindingFlags.Public);

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

        public static readonly List<Type> s_Types = new() { typeof(object), typeof(int), typeof(string) };

        private SerializedProperty m_ListProperty;

        private NewNodeView m_NodeView;

        private static VisualElement MakeItem()
        {
            VisualElement container = new();
            PopupField<Type> popupField = new("选择类型", s_Types, typeof(object));
            container.Add(popupField);
            return container;
        }

        private void BindElement(VisualElement element, int index)
        {
            SerializedProperty property = m_ListProperty.GetArrayElementAtIndex(index);
            if (property.managedReferenceValue is BasePort port)
            {
                element.Add(port.CreatePortContentView(property, out NewPortView portView));
                m_NodeView.AddPortView(portView);
            }
        }

        public override void InitNodeViewInfo()
        {
            m_LogicInput.InitPortViewInfo(nameof(m_LogicInput), "输入", Direction.Input, Port.Capacity.Multi, Color.red);
            for (int i = 0; i < LogContextInput.Count; i++)
            {
                LogContextInput[i].InitPortViewInfo(null, "日志输入", default, default, default);
            }
        }

        public override void FillNodeContentView(SerializedProperty property, NewNodeView nodeView)
        {
            base.FillNodeContentView(property, nodeView);
            m_ListProperty = property.FindPropertyRelative(nameof(LogContextInput));
            ListView listView = new(LogContextInput, 20, MakeItem, BindElement)
            {
                headerTitle = "输出日志",
                showFoldoutHeader = true,
                showBorder = true,
                showAddRemoveFooter = true,
            };
            nodeView.inputContainer.Add(listView);
        }
    }
}
#endif