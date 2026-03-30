using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace YBFramework.MyEditor.Common
{
    public sealed class DerivedTypePopupField : PopupField<string>
    {
        private readonly List<string> m_DerivedTypeNames;

        private readonly List<Type> m_DerivedTypes;

        private string m_SelectedTypeName;

        private Type m_SelectedType;

        private Action<Type> m_TypeChangedCallback;

        public DerivedTypePopupField(string label, List<string> derivedTypeNames, List<Type> derivedTypes) : base(label)
        {
            m_DerivedTypeNames = derivedTypeNames;
            m_DerivedTypes = derivedTypes;
            choices = derivedTypeNames;
            this.RegisterValueChangedCallback(OnSelectedTypeChanged);
        }

        public Type GetSelectedType()
        {
            return m_SelectedType;
        }
        
        public void Initialize(Type selectedType)
        {
            m_SelectedTypeName = ArrayUtility.FindValueByOtherIndex(m_DerivedTypes, m_DerivedTypeNames, selectedType);
            m_SelectedType = selectedType;
            SetValueWithoutNotify(m_SelectedTypeName);
        }
        
        public void RegisterTypeChangedCallBack(Action<Type> callback)
        {
            m_TypeChangedCallback += callback;
        }

        public void UnregisterTypeChangedCallBack(Action<Type> callback)
        {
            m_TypeChangedCallback -= callback;
        }
        
        private void OnSelectedTypeChanged(ChangeEvent<string> evt)
        {
            m_SelectedTypeName = evt.newValue;
            m_SelectedType = ArrayUtility.FindValueByOtherIndex(m_DerivedTypeNames, m_DerivedTypes, m_SelectedTypeName);
            m_TypeChangedCallback?.Invoke(m_SelectedType);
        }
    }
}