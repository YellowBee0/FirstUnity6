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

        private BuffLayer m_BuffLayer;

        private Buff m_Buff;

        [SerializeField] private float m_TotalTime;

        [SerializeField] private int m_ModifyLayer;

        private void ChangeLayer()
        {
            m_BuffLayer.ModifyCurValue("buff layer time", m_ModifyLayer);
            if (m_BuffLayer.GetCurValue() <= m_BuffLayer.GetMinValue())
            {
                m_Buff.GetManager().RemoveBuff(m_Buff);
            }
        }

        public void OnAdd(Buff buff)
        {
            m_BuffLayer = buff.GetComponent<BuffLayer>();
            if (m_BuffLayer == null)
            {
                buff.RemoveComponent(this);
            }
            else
            {
                m_Buff = buff;
                m_Timer.RegisterCallback(ChangeLayer);
                m_Timer.Start();
            }
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
            Timer timer = Timer.Allocate();
            timer.SetTotalTime(m_TotalTime);
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