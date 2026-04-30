using System;
using System.Collections.Generic;
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
#if UNITY_EDITOR
    [NodeMenu("Test/获取值", GraphType.Everything)]
#endif
    public sealed class GetValueNode : BaseNode
    {
        [Serializable]
        private class ValueWrapper<T>
        {
            public T Value;
        }

        private static readonly MethodInfo s_GetValueMethod = typeof(GetValueNode).GetMethod(nameof(GetIntValue), BindingFlags.Instance | BindingFlags.NonPublic);

        private static readonly Dictionary<Type, MethodInfo> s_GetValueMethodCache = new();

        [SerializeField] private MethodPort m_ValueOutput = new();

        [SerializeReference] private object m_ValueWrapper;

        private T GetIntValue<T>()
        {
            return ((ValueWrapper<T>)m_ValueWrapper).Value;
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

        public override void InitPortInfo()
        {
            if (m_ValueWrapper != null)
            {
                Type wrapperType = m_ValueWrapper.GetType();
                if (!s_GetValueMethodCache.TryGetValue(wrapperType, out MethodInfo methodInfo))
                {
                    Type valueType = wrapperType.GetGenericArguments()[0];
                    methodInfo = s_GetValueMethod.MakeGenericMethod(valueType);
                    s_GetValueMethodCache.Add(wrapperType, methodInfo);
                }
                m_ValueOutput.SetMethodInfo(methodInfo);
            }
        }
#if UNITY_EDITOR
        private PropertyField m_PropertyField;

        private SerializedProperty m_ValueProperty;

        public override void InitNodeViewInfo()
        {
            InitPortInfo();
            m_ValueOutput.InitPortViewInfo(nameof(m_ValueOutput), "值输出", Direction.Output, Port.Capacity.Multi, Color.blue);
        }

        public override void FillNodeContentView(SerializedProperty property, NodeView nodeView)
        {
            m_ValueOutput.CreatePortContentView(null, out PortView portView);
            VisualElement container = new();
            PopupField<Type> popupField = new("选择类型", DebugNode.s_Types, typeof(object));
            popupField.styleSheets.Add(StyleSheetManager.LoadStylesheet("Label"));
            popupField.RegisterValueChangedCallback(OnSelectedTypeChanged);
            container.Add(portView);
            container.Add(popupField);
            m_ValueProperty = property.FindPropertyRelative(nameof(m_ValueWrapper));
            if (m_ValueProperty != null)
            {
                m_PropertyField = new PropertyField
                {
                    label = "封装对象"
                };
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
                m_ValueOutput.SetMethodInfo(s_GetValueMethod.MakeGenericMethod(newType));
                m_ValueProperty.serializedObject.Update();
                m_ValueProperty.managedReferenceValue = Activator.CreateInstance(typeof(ValueWrapper<>).MakeGenericType(newType));
                m_ValueProperty.serializedObject.ApplyModifiedProperties();
            }
        }
#endif
    }
}