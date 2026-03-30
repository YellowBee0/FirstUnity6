using System;
using UnityEngine;
using YBFramework.Common;

namespace YBFramework.Component
{
#if UNITY_EDITOR
    [DisplayName("buff层数持续时间")]
#endif
    [Serializable]
    public sealed class BuffLayerTime : IBuffComponent, IPooledObject
    {
        private Timer m_Timer;

        private Buff m_Buff;

        [SerializeField] private float m_TotalTime;

        [SerializeField] private int m_ModifyLayer;

        [SerializeField] private bool m_RemoveBuffOnMinLayer;

        [SerializeField] private bool m_RemoveBuffOnMaxLayer;

        private void ChangeLayer()
        {
            BuffLayer buffLayer = m_Buff.GetComponent<BuffLayer>();
            if (buffLayer != null)
            {
                buffLayer.ModifyCurValue("buff layer time", m_ModifyLayer);
                int curLayer = buffLayer.GetCurValue();
                if ((m_RemoveBuffOnMinLayer && curLayer <= buffLayer.GetMinValue()) || (m_RemoveBuffOnMaxLayer && curLayer >= buffLayer.GetMaxValue()))
                {
                    m_Buff.GetManager().RemoveBuff(m_Buff);
                }
            }
        }

        public void OnAdd(Buff buff)
        {
            m_Buff = buff;
            m_Timer.RegisterCallback(ChangeLayer);
            m_Timer.Start();
        }

        public void OnRemove()
        {
            ObjectPool.Free(this);
        }

        public void OnReset()
        {
            m_Timer.Reset();
        }

        public void OnMagnificationChanged()
        {
            //TODO:倍率计算需要公式
            float magnification = m_Buff.GetMagnification();
            m_TotalTime *= magnification;
            m_ModifyLayer = (int)(m_ModifyLayer * magnification);
        }

        public IBuffComponent Clone()
        {
            BuffLayerTime buffLayerTime = ObjectPool.Allocate<BuffLayerTime>();
            Timer timer = Timer.Allocate(m_TotalTime);
            timer.SetIsLoop(true);
            buffLayerTime.m_Timer = timer;
            buffLayerTime.m_TotalTime = m_TotalTime;
            buffLayerTime.m_ModifyLayer = m_ModifyLayer;
            return buffLayerTime;
        }

        public void OnFree()
        {
            Timer.Free(m_Timer);
        }
    }
}