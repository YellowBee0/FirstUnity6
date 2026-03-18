using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace YBFramework.MyEditor.Common
{
    public sealed class DynamicElementView : VisualElement
    {
        private readonly PopupField<Type> m_TypeSelectField;

        private readonly PropertyField m_ElementField;

        private SerializedProperty m_Property;

        public DynamicElementView(List<Type> typeOptions)
        {
            style.flexDirection = FlexDirection.Column;
            m_TypeSelectField = new PopupField<Type>("选择类型")
            {
                choices = typeOptions
            };
            m_TypeSelectField.RegisterValueChangedCallback(OnTypeChanged);
            m_ElementField = new PropertyField();
            Add(m_TypeSelectField);
            Add(m_ElementField);
        }

        public void Bind(SerializedProperty property)
        {
            m_Property = property;
            Type defaultType = null;
            if (m_Property.managedReferenceValue != null)
            {
                defaultType = m_Property.managedReferenceValue.GetType();
            }
            m_TypeSelectField.SetValueWithoutNotify(defaultType);
            m_ElementField.BindProperty(m_Property);
        }

        private void OnTypeChanged(ChangeEvent<Type> evt)
        {
            m_Property.serializedObject.Update();
            m_Property.managedReferenceValue = Activator.CreateInstance(evt.newValue);
            m_Property.serializedObject.ApplyModifiedProperties();
        }
    }
}