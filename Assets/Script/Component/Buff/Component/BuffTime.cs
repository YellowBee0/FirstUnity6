using System;
using UnityEngine;
using YBFramework.Common;

namespace YBFramework.Component
{
#if UNITY_EDITOR
    [DisplayName("buff持续时间")]
#endif
    [Serializable]
    public sealed class BuffTime : IBuffComponent, IPooledObject
    {
        private Buff m_Buff;

        private Timer m_Timer;

        [SerializeField] private float m_TotalTime;

        private void RemoveSelfBuff()
        {
            m_Buff.GetManager().RemoveBuff(m_Buff);
        }

        public void OnAdd(Buff buff)
        {
            m_Buff = buff;
            m_Timer.RegisterCallback(RemoveSelfBuff);
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
            m_TotalTime *= m_Buff.GetMagnification();
            m_Timer.SetTotalTime(m_TotalTime);
        }

        public IBuffComponent Clone()
        {
            BuffTime buffTime = ObjectPool.Allocate<BuffTime>();
            buffTime.m_Timer = Timer.Allocate(m_TotalTime);
            return buffTime;
        }

        public void OnFree()
        {
            Timer.Free(m_Timer);
        }
    }
}