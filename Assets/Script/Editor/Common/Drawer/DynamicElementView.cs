using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace YBFramework.MyEditor.Common
{
    public sealed class DynamicElementView : VisualElement
    {
        private readonly PopupField<Type> TypeSelectField;

        private readonly PropertyField ElementField;

        private SerializedProperty m_ListProperty;

        private int m_Index;

        public DynamicElementView(List<Type> typeOptions)
        {
            style.flexDirection = FlexDirection.Column;
            TypeSelectField = new PopupField<Type>("选择类型")
            {
                choices = typeOptions
            };
            TypeSelectField.RegisterValueChangedCallback(OnTypeChanged);
            ElementField = new PropertyField();
            Add(TypeSelectField);
            Add(ElementField);
        }

        public void Bind(SerializedProperty listProperty, int index)
        {
            m_ListProperty = listProperty;
            m_Index = index;
            SerializedProperty property = m_ListProperty.GetArrayElementAtIndex(index);
            Type defaultType = null;
            if (property.managedReferenceValue != null)
            {
                defaultType = property.managedReferenceValue.GetType();
            }
            TypeSelectField.value = defaultType;
            ElementField.Unbind();
            ElementField.BindProperty(property);
        }

        private void OnTypeChanged(ChangeEvent<Type> evt)
        {
            ElementField.Unbind();
            m_ListProperty.serializedObject.Update();
            SerializedProperty serializedProperty = m_ListProperty.GetArrayElementAtIndex(m_Index);
            serializedProperty.managedReferenceValue = Activator.CreateInstance(evt.newValue);
            m_ListProperty.serializedObject.ApplyModifiedProperties();
            ElementField.BindProperty(serializedProperty);
        }
    }
}