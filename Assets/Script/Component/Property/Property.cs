using System;
using UnityEngine;
using YBFramework.Common;

namespace YBFramework.Component
{
    [Serializable]
    public sealed class Property : BaseValue<float>
    {
        public delegate void Preprocess(string modifier, ref float value);

        [SerializeField] private PropertyType m_PropertyType;

        private Action m_OnCurValueChanged;

        private Action m_OnMaxValueChanged;

        private Action m_OnMinValueChanged;

        private Preprocess m_Preprocess;

        public override void CopyFrom(BaseValue<float> other)
        {
            base.CopyFrom(other);
            Property otherProperty = (Property)other;
            m_PropertyType = otherProperty.m_PropertyType;
        }

        public override void ModifyMaxValue(string modifier, float delta)
        {
            if (delta == 0)
            {
                return;
            }
            float percent = (m_MaxValue - m_CurValue) / (m_MaxValue - m_MinValue);
            float oldValue = m_MaxValue;
            float newValue = m_MaxValue + delta;
            if (newValue < m_MinValue)
            {
                newValue = m_MinValue;
            }
            m_MaxValue = newValue;
            m_OnMaxValueChanged?.Invoke();
            float actualModifiedValue = newValue - oldValue;
            Record(ValueConstraintType.Max, modifier, delta, actualModifiedValue);
            ModifyCurValue(modifier + "(self max value)", percent * actualModifiedValue);
        }

        public override void ModifyMinValue(string modifier, float delta)
        {
            if (delta == 0)
            {
                return;
            }
            float percent = (m_MaxValue - m_CurValue) / (m_MaxValue - m_MinValue);
            float oldValue = m_MinValue;
            float newValue = m_MinValue + delta;
            if (newValue > m_MaxValue)
            {
                newValue = m_MaxValue;
            }
            m_MinValue = newValue;
            m_OnMinValueChanged?.Invoke();
            float actualModifiedValue = newValue - oldValue;
            Record(ValueConstraintType.Min, modifier, delta, actualModifiedValue);
            ModifyCurValue(modifier + "(self min value)", percent * actualModifiedValue);
        }

        public override void ModifyCurValue(string modifier, float delta)
        {
            if (delta == 0)
            {
                return;
            }
            float oldValue = m_CurValue;
            float newValue = m_CurValue + delta;
            m_Preprocess?.Invoke(modifier, ref newValue);
            if (newValue > m_MaxValue)
            {
                newValue = m_MaxValue;
            }
            else if (newValue < m_MinValue)
            {
                newValue = m_MinValue;
            }
            m_CurValue = newValue;
            m_OnCurValueChanged?.Invoke();
            Record(ValueConstraintType.Current, modifier, delta, oldValue - newValue);
        }

        public PropertyType GetPropertyType()
        {
            return m_PropertyType;
        }

        public void RegisterValueChangeCallBack(ValueConstraintType valueConstraintType, Action callBack)
        {
            switch (valueConstraintType)
            {
                case ValueConstraintType.Max:
                    m_OnMaxValueChanged += callBack;
                    return;
                case ValueConstraintType.Min:
                    m_OnMinValueChanged += callBack;
                    return;
                case ValueConstraintType.Current:
                    m_OnCurValueChanged += callBack;
                    return;
            }
        }

        public void UnregisterValueChangeCallBack(ValueConstraintType valueConstraintType, Action callBack)
        {
            switch (valueConstraintType)
            {
                case ValueConstraintType.Max:
                    m_OnMaxValueChanged -= callBack;
                    return;
                case ValueConstraintType.Min:
                    m_OnMinValueChanged -= callBack;
                    return;
                case ValueConstraintType.Current:
                    m_OnCurValueChanged -= callBack;
                    return;
            }
        }

        public void RegisterPreprocess(Preprocess preprocess)
        {
            m_Preprocess += preprocess;
        }

        public void UnregisterPreprocess(Preprocess preprocess)
        {
            m_Preprocess -= preprocess;
        }

        public override void OnFree()
        {
            base.OnFree();
            m_OnMaxValueChanged = null;
            m_OnMinValueChanged = null;
            m_OnCurValueChanged = null;
            m_Preprocess = null;
        }
    }
}