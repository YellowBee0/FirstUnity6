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

        [SerializeReference] private object m_Value = new string("test");

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
            m_ValueOutput.CreatePortContentView(null, out NewPortView portView);
            VisualElement container = new();
            PopupField<Type> popupField = new("选择类型", DebugNode.s_Types, typeof(object));
            popupField.styleSheets.Add(StyleSheetManager.LoadStylesheet("Label"));
            popupField.RegisterValueChangedCallback(OnSelectedTypeChanged);
            container.Add(portView);
            container.Add(popupField);
            m_ValueProperty = property.FindPropertyRelative(nameof(m_Value));
            if (m_ValueProperty != null)
            {
                m_PropertyField = new PropertyField();
                m_PropertyField.styleSheets.Add(StyleSheetManager.LoadStylesheet("Label"));
                m_PropertyField.BindProperty(m_ValueProperty);
                container.Add(m_PropertyField);
            }
            if (m_ValueOutput.GetDirection() == Direction.Input)
            {
                nodeView.inputContainer.Add(container);
            }
            else
            {
                nodeView.outputContainer.Add(container);
            }
            nodeView.AddPortView(portView);
        }

        private void OnSelectedTypeChanged(ChangeEvent<Type> evt)
        {
            Type newType = evt.newValue;
            if (newType != null)
            {
                m_SelectedValueType = newType.Name;
                object instance = newType == typeof(string) ? new string("") : Activator.CreateInstance(newType);
                m_ValueOutput.SetMethodInfo(s_GetValueMethod.MakeGenericMethod(newType));
                m_ValueProperty.serializedObject.Update();
                m_ValueProperty.managedReferenceValue = instance;
                m_ValueProperty.serializedObject.ApplyModifiedProperties();
            }
            else
            {
                m_SelectedValueType = null;
            }
        }
#endif
    }
}