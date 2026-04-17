using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace YBFramework.MyEditor.Common
{
    public class ManagedReferencePropertyView : VisualElement
    {
        private readonly DerivedTypePopupField TypePopupField;

        private readonly PropertyField m_PropertyField;

        private readonly Type m_DefaultDerivedType;

        private SerializedProperty m_BindProperty;

        public ManagedReferencePropertyView(Type baseType, Type defaultDerivedType)
        {
            ValueTuple<List<string>, List<Type>> group = DerivedTypeManager.GetDerivedTypes(baseType);
            if (group != default)
            {
                m_DefaultDerivedType = defaultDerivedType;
                TypePopupField = new DerivedTypePopupField("select type", group.Item1, group.Item2);
                TypePopupField.RegisterTypeChangedCallBack(OnSelectedTypeChanged);
                m_PropertyField = new PropertyField();
                Add(TypePopupField);
                Add(m_PropertyField);
            }
        }

        private void OnSelectedTypeChanged(Type newType)
        {
            m_BindProperty.serializedObject.Update();
            object newValue = null;
            if (newType != null)
            {
                newValue = Activator.CreateInstance(newType);
            }
            m_BindProperty.managedReferenceValue = newValue;
            m_PropertyField.BindProperty(m_BindProperty);
            m_BindProperty.serializedObject.ApplyModifiedProperties();
        }

        public void Bind(SerializedProperty property)
        {
            m_BindProperty = property;
            if (m_BindProperty.managedReferenceValue == null)
            {
                TypePopupField.Initialize(m_DefaultDerivedType);
                Type type = TypePopupField.GetSelectedType();
                if (type != null)
                {
                    m_BindProperty.managedReferenceValue = Activator.CreateInstance(type);
                }
            }
            else
            {
                TypePopupField.Initialize(m_BindProperty.managedReferenceValue.GetType());
            }
            m_PropertyField.BindProperty(m_BindProperty);
        }
    }
}