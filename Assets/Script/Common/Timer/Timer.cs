using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace YBFramework.Common
{
    public sealed class Timer
    {
        private static readonly Queue<Timer> s_Pool = new();

        public static Timer Allocate()
        {
            return s_Pool.Count > 0 ? s_Pool.Dequeue() : new Timer();
        }

        public static void Free(Timer timer)
        {
            timer.Stop();
            timer.m_IsLoop = false;
            timer.m_Callback = null;
            s_Pool.Enqueue(timer);
        }

        private Action m_Callback;

        private bool m_IsLoop;

        private bool m_IsRunning;

        private Timer m_Next;

        private Timer m_Prev;

        private float m_Timer;

        private float m_TotalTime;

        private Timer()
        {
        }

        private void Update(float deltaTime)
        {
            if (m_Timer >= m_TotalTime)
            {
                m_Callback.Invoke();
                if (m_IsLoop)
                {
                    m_Timer = 0;
                }
                else
                {
                    Stop();
                    return;
                }
            }
            m_Timer += deltaTime;
        }

        public void SetTotalTime(float totalTime)
        {
            if (totalTime <= 0)
            {
                throw new ArgumentException("Invalid total time");
            }
            m_TotalTime = totalTime;
        }

        public void RegisterCallback(Action callback)
        {
            m_Callback += callback ?? throw new ArgumentException("Invalid callback");
        }

        public void UnregisterCallback(Action callback)
        {
            m_Callback -= callback;
            if (m_Callback == null)
            {
                Stop();
            }
        }

        public void SetIsLoop(bool isLoop)
        {
            m_IsLoop = isLoop;
        }

        public void Start()
        {
            if (m_IsRunning)
            {
                return;
            }
            if (m_TotalTime <= 0 || m_Callback == null)
            {
                throw new InvalidOperationException("Timer is not properly initialized");
            }
            TimerMonitor.StartTimer(this);
            m_IsRunning = true;
        }

        public void Stop()
        {
            if (m_IsRunning)
            {
                TimerMonitor.StopTimer(this);
                m_IsRunning = false;
            }
        }

        public void Reset()
        {
            m_Timer = 0;
        }

        private static class TimerMonitor
        {
            private static Timer s_CurTimer;

            public static void StartTimer(Timer timer)
            {
                if (timer == null)
                {
                    return;
                }
                Timer curTimer = s_CurTimer;
                s_CurTimer = timer;
                timer.m_Prev = null;
                if (curTimer == null)
                {
                    timer.m_Next = null;
                    Update().Forget();
                }
                else
                {
                    timer.m_Next = curTimer;
                    curTimer.m_Prev = timer;
                }
            }

            public static void StopTimer(Timer timer)
            {
                if (timer == null)
                {
                    return;
                }
                if (timer.m_Prev != null)
                {
                    timer.m_Prev.m_Next = timer.m_Next;
                }
                else
                {
                    s_CurTimer = s_CurTimer.m_Next;
                }
                if (timer.m_Next != null)
                {
                    timer.m_Next.m_Prev = timer.m_Prev;
                }
            }

            private static async UniTaskVoid Update()
            {
                while (s_CurTimer != null)
                {
                    Timer timer = s_CurTimer;
                    while (timer != null)
                    {
                        timer.Update(Time.deltaTime);
                        timer = timer.m_Next;
                    }
                    await UniTask.NextFrame();
                }
            }
        }
    }
}