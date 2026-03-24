using System;
using YBFramework.Common;

namespace YBFramework.Component
{
#if UNITY_EDITOR
    [DisplayName("buff层数")]
#endif
    [Serializable]
    public sealed class BuffLayer : BaseValue<int>, IBuffComponent
    {
        private Buff m_Buff;

        private Action m_OnLayerChanged;

        public override void ModifyMaxValue(string modifier, int delta)
        {
            if (delta == 0)
            {
                return;
            }
            int oldValue = m_MaxValue;
            int newValue = m_MaxValue + delta;
            if (newValue < m_MinValue) newValue = m_MinValue;
            m_MaxValue = newValue;
            Record(ValueConstraintType.Max, modifier, delta, newValue - oldValue);
            if (m_CurValue > m_MaxValue)
            {
                ModifyCurValue(modifier + "(self max value)", m_MaxValue - m_CurValue);
            }
        }

        public override void ModifyMinValue(string modifier, int delta)
        {
            if (delta == 0)
            {
                return;
            }
            int oldValue = m_MaxValue;
            int newValue = m_MaxValue + delta;
            if (newValue > m_MaxValue) newValue = m_MaxValue;
            m_MinValue = newValue;
            Record(ValueConstraintType.Min, modifier, delta, newValue - oldValue);
            if (m_CurValue < m_MinValue)
            {
                ModifyCurValue(modifier + "(self min value)", m_MinValue - m_CurValue);
            }
        }

        public override void ModifyCurValue(string modifier, int delta)
        {
            if (delta == 0)
            {
                return;
            }
            int oldValue = m_CurValue;
            int newValue = m_CurValue + delta;
            if (newValue > m_MaxValue)
            {
                newValue = m_MaxValue;
            }
            else if (newValue < m_MinValue)
            {
                newValue = m_MinValue;
            }
            m_CurValue = newValue;
            m_OnLayerChanged?.Invoke();
            Record(ValueConstraintType.Current, modifier, delta, oldValue - newValue);
        }

        public void RegisterLayerChangeCallBack(Action callBack)
        {
            m_OnLayerChanged += callBack;
        }

        public void UnregisterLayerChangeCallBack(Action callBack)
        {
            m_OnLayerChanged -= callBack;
        }

        public override void OnFree()
        {
            base.OnFree();
            m_OnLayerChanged = null;
        }

        public void OnReset()
        {
            ModifyCurValue("Reset", m_MinValue - m_MaxValue);
        }

        public void OnAdd(Buff buff)
        {
            m_Buff = buff;
        }

        public void OnRemove()
        {
            ObjectPool.Free(this);
        }

        public void OnMagnificationChanged()
        {
            ModifyCurValue("Buff magnification changed", (int)(m_CurValue * (m_Buff.GetMagnification() - 1)));
        }

        public IBuffComponent Clone()
        {
            BuffLayer layer = ObjectPool.Allocate<BuffLayer>();
            layer.CopyFrom(this);
            return layer;
        }
    }
}