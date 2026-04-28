using System;
using System.Reflection;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using YBFramework.MyEditor;
#endif

namespace YBFramework.Component
{
    [Serializable]
    [NodeMenu("Test/获取值", GraphType.Everything)]
    public sealed class GetValueNode : BaseNode
    {
        private static readonly MethodInfo s_GetValueMethod = typeof(GetValueNode).GetMethod(nameof(GetIntValue), BindingFlags.Instance | BindingFlags.NonPublic);

        [SerializeField] private MethodPort m_ValueOutput = new(s_GetValueMethod);

        [SerializeReference] private object m_Value;

        private T GetIntValue<T>()
        {
            return (T)m_Value;
        }

        protected override BasePort PortIterator(int index)
        {
            switch (index)
            {
                case 0:
                    return m_ValueOutput;
                default:
                    return null;
            }
        }

        public override BaseNode Clone()
        {
            GetValueNode node = new()
            {
                ID = ID
            };
            CopyPort(this, node);
            return node;
        }

#if UNITY_EDITOR
        [SerializeField] private string m_SelectedValueType;

        private PropertyField m_PropertyField;

        private SerializedProperty m_ValueProperty;

        public override void InitNodeViewInfo()
        {
            m_ValueOutput.InitPortViewInfo(nameof(m_ValueOutput), "值输出", Direction.Output, Port.Capacity.Multi, Color.blue);
        }

        public override void FillNodeContentView(SerializedProperty property, NewNodeView nodeView)
        {
            base.FillNodeContentView(property, nodeView);
            PopupField<Type> popupField = new();
            popupField.RegisterValueChangedCallback(OnSelectedTypeChanged);
            m_ValueProperty = property.FindPropertyRelative(nameof(m_Value));
            m_PropertyField = new PropertyField(m_ValueProperty);
            nodeView.extensionContainer.Add(popupField);
            nodeView.extensionContainer.Add(m_PropertyField);
        }

        private void OnSelectedTypeChanged(ChangeEvent<Type> evt)
        {
            m_SelectedValueType = evt.newValue.Name;
            Type newType = Type.GetType(m_SelectedValueType);
            if (newType != null)
            {
                m_ValueOutput.SetMethodInfo(s_GetValueMethod.MakeGenericMethod(newType));
                m_ValueProperty.managedReferenceValue = Activator.CreateInstance(newType);
            }
        }
#endif
    }
}