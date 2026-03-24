using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using YBFramework.Common;

namespace YBFramework.MyEditor.Common
{
    public class ManagedReferencePropertyView : VisualElement
    {
        private static readonly Dictionary<Type, ValueTuple<List<Type>, List<string>>> s_DerivedClasses = new();

        private static ValueTuple<List<Type>, List<string>> RegisterDerivedMap(Type type, Type defaultType)
        {
            if (!s_DerivedClasses.TryGetValue(type, out ValueTuple<List<Type>, List<string>> derivedClasses))
            {
                List<Type> derivedTypes = new();
                List<string> derivedDisplayNames = new();
                bool isAdd = false;
                TypeCache.TypeCollection types = TypeCache.GetTypesDerivedFrom(type);
                foreach (Type derivedType in types)
                {
                    if (!derivedType.IsAbstract)
                    {
                        DisplayNameAttribute displayName = derivedType.GetCustomAttribute<DisplayNameAttribute>();
                        if (defaultType == derivedType)
                        {
                            derivedTypes.Insert(0, derivedType);
                            derivedDisplayNames.Insert(0, displayName != null ? displayName.Name : derivedType.Name);
                            isAdd = true;
                        }
                        else
                        {
                            derivedTypes.Add(derivedType);
                            derivedDisplayNames.Add(displayName != null ? displayName.Name : derivedType.Name);
                        }
                    }
                }
                if (!isAdd)
                {
                    derivedTypes.Insert(0, null);
                    derivedDisplayNames.Insert(0, "空");
                }
                derivedClasses = new ValueTuple<List<Type>, List<string>>(derivedTypes, derivedDisplayNames);
                s_DerivedClasses.Add(type, derivedClasses);
            }
            return derivedClasses;
        }

        private static ValueTuple<List<Type>, List<string>> GetDerivedMap(Type type)
        {
            return s_DerivedClasses.GetValueOrDefault(type);
        }

        private readonly PopupField<string> m_TypeSelectField;

        private readonly PropertyField m_ElementField;

        private readonly Type m_BaseType;

        private SerializedProperty m_Property;

        public ManagedReferencePropertyView(Type baseType, Type defaultDerivedType)
        {
            if (baseType != null)
            {
                m_BaseType = baseType;
                List<string> derivedDisplayNames = RegisterDerivedMap(baseType, defaultDerivedType).Item2;
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
            ValueTuple<List<Type>, List<string>> derivedMap = GetDerivedMap(m_BaseType);
            if (m_Property.managedReferenceValue == null)
            {
                string displayName = ArrayUtility.FindValueByOtherIndex(derivedMap.Item1, derivedMap.Item2, derivedMap.Item1[0]);
                m_TypeSelectField.value = displayName;
                return;
            }
            m_TypeSelectField.SetValueWithoutNotify(ArrayUtility.FindValueByOtherIndex(derivedMap.Item1, derivedMap.Item2, m_Property.managedReferenceValue.GetType()));
            m_ElementField.BindProperty(m_Property);
        }

        private void OnValueChanged(ChangeEvent<string> evt)
        {
            m_Property.serializedObject.Update();
            ValueTuple<List<Type>, List<string>> derivedMap = GetDerivedMap(m_BaseType);
            Type newType = ArrayUtility.FindValueByOtherIndex(derivedMap.Item2, derivedMap.Item1, evt.newValue);
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