using System;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using YBFramework.Component;
using YBFramework.MyEditor.Common;
using Object = UnityEngine.Object;

namespace YBFramework.MyEditor
{
    [Drawer(typeof(FuncPort<>))]
    public sealed class FuncPortDrawer<TValue> : CommonPortDrawer
    {
        private FuncPort<TValue> m_Port;

        private NodeView m_NodeView;

        public override VisualElement DrawPortView(NodeView nodeView, BasePort target)
        {
            m_NodeView = nodeView;
            m_Port = target as FuncPort<TValue>;
            if (m_Port != null)
            {
                VisualElement portView = base.DrawPortView(nodeView, target);
                VisualElement container = portView;
                VisualElement bindableElement = null;
                Type drawValueType = typeof(TValue);
                bool isSpecialType = false;
                if (drawValueType != typeof(object))
                {
                    if (drawValueType == typeof(bool))
                    {
                        bindableElement = new Toggle();
                    }
                    else if (drawValueType == typeof(int))
                    {
                        bindableElement = new IntegerField();
                    }
                    else if (drawValueType == typeof(float))
                    {
                        bindableElement = new FloatField();
                    }
                    else if (drawValueType == typeof(double))
                    {
                        bindableElement = new DoubleField();
                    }
                    else if (drawValueType == typeof(Color))
                    {
                        bindableElement = new ColorField();
                    }
                    else if (drawValueType == typeof(string))
                    {
                        bindableElement = new TextField();
                    }
                    else if (drawValueType == typeof(AnimationCurve))
                    {
                        bindableElement = new CurveField();
                    }
                    else if (typeof(Object).IsAssignableFrom(drawValueType))
                    {
                        isSpecialType = true;
                        ObjectField field = new()
                        {
                            objectType = drawValueType,
                            value = (Object)(object)m_Port.GetValue()
                        };
                        field.RegisterValueChangedCallback(OnObjectValueChanged);
                        bindableElement = field;
                    }
                    else if (drawValueType.IsEnum)
                    {
                        isSpecialType = true;
                        BaseField<Enum> field;
                        Enum value = (Enum)Enum.ToObject(drawValueType, m_Port.GetValue());
                        if (drawValueType.IsDefined(typeof(FlagsAttribute), false))
                        {
                            field = new EnumFlagsField(value);
                        }
                        else
                        {
                            field = new EnumField(value);
                        }
                        field.RegisterValueChangedCallback(OnEnumValueChanged);
                        bindableElement = field;
                    }
                }
                if (bindableElement != null)
                {
                    if (!isSpecialType)
                    {
                        BaseField<TValue> field = (BaseField<TValue>)bindableElement;
                        field.value = m_Port.GetValue();
                        field.RegisterValueChangedCallback(OnValueChanged);
                    }
                    container = new VisualElement();
                    container.Add(portView);
                    container.Add(bindableElement);
                }
                return container;
            }
            return null;
        }

        private void OnValueChanged(ChangeEvent<TValue> evt)
        {
            m_Port.SetValue(evt.newValue);
            m_NodeView.NodeAsset.SetSelfDirty();
        }

        private void OnObjectValueChanged(ChangeEvent<Object> evt)
        {
            m_Port.SetValue((TValue)(object)evt.newValue);
            m_NodeView.NodeAsset.SetSelfDirty();
        }

        private void OnEnumValueChanged(ChangeEvent<Enum> evt)
        {
            m_Port.SetValue((TValue)Enum.ToObject(typeof(TValue), evt.newValue));
            m_NodeView.NodeAsset.SetSelfDirty();
        }
    }
}