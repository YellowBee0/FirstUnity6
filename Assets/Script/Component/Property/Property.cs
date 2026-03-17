using System;
using YBFramework.Common;

namespace YBFramework.Component
{
    public sealed class Property : BaseValue<float>
    {
        #region Delegates
        public delegate void Preprocess(string modifier, ref float value);
        #endregion

        private bool m_IsPreprocessIncrease;

        private bool m_IsPreprocessReduce;

        private Action m_OnCurValueChanged;

        private Action m_OnMaxValueChanged;

        private Action m_OnMinValueChanged;

        private Preprocess m_PreprocessIncrease;

        private Preprocess m_PreprocessReduce;

        public override void ModifyMaxValue(string modifier, float delta)
        {
            if (delta == 0) return;
            var percent = (m_MaxValue - m_CurValue) / (m_MaxValue - m_MinValue);
            var oldValue = m_MaxValue;
            var newValue = m_MaxValue + delta;
            if (newValue < m_MinValue) newValue = m_MinValue;
            m_MaxValue = newValue;
            m_OnMaxValueChanged?.Invoke();
            var actualModifiedValue = newValue - oldValue;
            Record(ValueConstraintType.Max, modifier, delta, actualModifiedValue);
            //TODO 字符串本地化
            ModifyCurValue(modifier + "(self max value)", percent * actualModifiedValue);
        }

        public override void ModifyMinValue(string modifier, float delta)
        {
            if (delta == 0) return;
            var percent = (m_MaxValue - m_CurValue) / (m_MaxValue - m_MinValue);
            var oldValue = m_MinValue;
            var newValue = m_MinValue + delta;
            if (newValue > m_MaxValue) newValue = m_MaxValue;
            m_MinValue = newValue;
            m_OnMinValueChanged?.Invoke();
            var actualModifiedValue = newValue - oldValue;
            Record(ValueConstraintType.Min, modifier, delta, actualModifiedValue);
            //TODO 字符串本地化
            ModifyCurValue(modifier + "(self min value)", percent * actualModifiedValue);
        }

        public override void ModifyCurValue(string modifier, float delta)
        {
            if (delta == 0) return;
            var oldValue = m_CurValue;
            var newValue = m_CurValue + delta;
            if (delta > 0)
            {
                if (m_IsPreprocessIncrease) m_PreprocessIncrease.Invoke(modifier, ref newValue);
            }
            else
            {
                if (m_IsPreprocessReduce) m_PreprocessReduce.Invoke(modifier, ref newValue);
            }
            if (newValue > m_MaxValue)
                newValue = m_MaxValue;
            else if (newValue < m_MinValue) newValue = m_MinValue;
            m_CurValue = newValue;
            m_OnCurValueChanged?.Invoke();
            Record(ValueConstraintType.Current, modifier, delta, oldValue - newValue);
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

        public void RegisterPreprocess(bool isIncrease, Preprocess preprocess)
        {
            if (isIncrease)
            {
                if (!m_IsPreprocessIncrease) m_IsPreprocessIncrease = true;

                m_PreprocessIncrease += preprocess;
            }
            else
            {
                if (!m_IsPreprocessReduce) m_IsPreprocessReduce = true;

                m_PreprocessReduce += preprocess;
            }
        }

        public void UnregisterPreprocess(bool isIncrease, Preprocess preprocess)
        {
            if (isIncrease)
            {
                m_PreprocessIncrease -= preprocess;
                if (m_PreprocessIncrease == null) m_IsPreprocessIncrease = false;
            }
            else
            {
                m_PreprocessReduce -= preprocess;
                if (m_PreprocessReduce == null) m_IsPreprocessReduce = false;
            }
        }

        protected override void OnFree()
        {
            base.OnFree();
            m_OnMaxValueChanged = null;
            m_OnMinValueChanged = null;
            m_OnCurValueChanged = null;
            m_PreprocessIncrease = null;
            m_PreprocessReduce = null;
            m_IsPreprocessIncrease = false;
            m_IsPreprocessReduce = false;
        }
    }
}