using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace YBFramework.MyEditor.Common
{
    public class ManagedReferencePropertyView : VisualElement
    {
        private readonly DerivedTypePopupField m_TypePopupField;

        private readonly PropertyField m_ElementField;

        private readonly Type m_DefaultDerivedType;

        private SerializedProperty m_Property;

        public ManagedReferencePropertyView(Type baseType, Type defaultDerivedType)
        {
            ValueTuple<List<string>, List<Type>> group = DerivedTypeManager.GetDerivedTypes(baseType);
            if (group != default)
            {
                m_DefaultDerivedType = defaultDerivedType;
                m_TypePopupField = new DerivedTypePopupField("select type", group.Item1, group.Item2);
                m_TypePopupField.RegisterTypeChangedCallBack(OnSelectedTypeChanged);
                m_ElementField = new PropertyField();
                Add(m_TypePopupField);
                Add(m_ElementField);
            }
        }

        private void OnSelectedTypeChanged(Type newType)
        {
            m_Property.serializedObject.Update();
            object newValue = null;
            if (newType != null)
            {
                newValue = Activator.CreateInstance(newType);
            }
            m_Property.managedReferenceValue = newValue;
            m_ElementField.BindProperty(m_Property);
        }

        public void Bind(SerializedProperty property)
        {
            m_Property = property;
            if (m_Property.managedReferenceValue == null)
            {
                m_TypePopupField.Initialize(m_DefaultDerivedType);
                Type type = m_TypePopupField.GetSelectedType();
                if (type != null)
                {
                    m_Property.managedReferenceValue = Activator.CreateInstance(type);
                }
            }
            else
            {
                m_TypePopupField.Initialize(m_Property.managedReferenceValue.GetType());
            }
            m_ElementField.BindProperty(m_Property);
        }
    }
}