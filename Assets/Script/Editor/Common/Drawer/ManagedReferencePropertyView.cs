using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace YBFramework.MyEditor.Common
{
    public class ManagedReferencePropertyView : VisualElement
    {
        private readonly PopupField<string> m_TypeSelectField;

        private readonly PropertyField m_ElementField;

        private readonly Type m_BaseType;

        private SerializedProperty m_Property;

        public ManagedReferencePropertyView(Type baseType)
        {
            if (baseType != null)
            {
                m_BaseType = baseType;
                List<string> derivedDisplayNames = TypeCache.GetDerivedClasses(baseType).Item2;
                m_TypeSelectField = new PopupField<string>("选择类型")
                {
                    choices = derivedDisplayNames
                };
                m_TypeSelectField.RegisterValueChangedCallback(OnValueChanged);
                m_ElementField = new PropertyField();
                Add(m_TypeSelectField);
                Add(m_ElementField);
            }
            else
            {
                throw new ArgumentNullException();
            }
        }

        public void Bind(SerializedProperty property)
        {
            m_Property = property;
            m_TypeSelectField.SetValueWithoutNotify(TypeCache.GetDerivedClassDisplayName(m_BaseType, m_Property.managedReferenceValue?.GetType()));
            m_ElementField.BindProperty(m_Property);
        }

        private void OnValueChanged(ChangeEvent<string> evt)
        {
            m_Property.serializedObject.Update();
            Type newType = TypeCache.GetDerivedClassType(m_BaseType, evt.newValue);
            object newValue = null;
            if (newType != null)
            {
                newValue = Activator.CreateInstance(newType);
            }
            m_Property.managedReferenceValue = newValue;
            m_Property.serializedObject.ApplyModifiedProperties();
            m_ElementField.BindProperty(m_Property);
        }
    }
}