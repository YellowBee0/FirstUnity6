using UnityEngine;
using YBFramework.Common;

namespace YBFramework.Component
{
    public abstract class TimedBehaviour : BuffBehaviour
    {
        [SerializeField] protected float m_Interval;

        protected Timer m_Timer;

        public override void OnMagnificationChanged()
        {
            m_Interval *= m_BUFF.GetMagnification();
            m_Timer.SetTotalTime(m_Interval);
        }

        public override void OnStart()
        {
            m_Timer.Start();
        }

        public override void OnStop()
        {
            m_Timer.Stop();
        }

        public override void OnRemove()
        {
            base.OnRemove();
            m_Timer.Reset();
        }

        public override void OnReset()
        {
            m_Timer.Reset();
        }
    }
}