using System;
using UnityEngine;
using YBFramework.Common;

namespace YBFramework.Component
{
#if UNITY_EDITOR
    [DisplayName("固定时间执行任务")]
#endif
    [Serializable]
    public sealed class TimeExecutor : IExecutor
    {
        private Timer m_Timer;

        [SerializeField] private float m_TotalTime;

        [SerializeField] private bool m_IsLoop;

        public void Initialize(Entity entity)
        {
        }

        public void Start()
        {
            m_Timer.Start();
        }

        public void Stop()
        {
            m_Timer.Stop();
        }

        public void Reset()
        {
            m_Timer.Reset();
        }

        public void RegisterExecuteCallback(Action callback)
        {
            m_Timer.RegisterCallback(callback);
        }

        public void UnregisterExecuteCallback(Action callback)
        {
            m_Timer.RegisterCallback(callback);
        }

        public IExecutor Clone()
        {
            TimeExecutor executor = new()
            {
                m_Timer = Timer.Allocate(),
                m_TotalTime = m_TotalTime,
                m_IsLoop = m_IsLoop
            };
            executor.m_Timer.SetTotalTime(m_TotalTime);
            executor.m_Timer.SetIsLoop(m_IsLoop);
            return executor;
        }
    }
}