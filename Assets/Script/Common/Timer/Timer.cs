using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace YBFramework.Common
{
    public sealed class Timer
    {
        private static readonly Queue<Timer> s_Pool = new();

        public static Timer Allocate(float totalTime)
        {
            Timer timer = s_Pool.Count > 0 ? s_Pool.Dequeue() : new Timer();
            timer.SetTotalTime(totalTime);
            return timer;
        }

        public static void Free(Timer timer)
        {
            timer.Stop();
            timer.m_Timer = 0;
            timer.m_IsLoop = false;
            timer.m_Callback = null;
            s_Pool.Enqueue(timer);
        }

        private Action m_Callback;

        private float m_TotalTime;

        private float m_Timer;

        private int m_Index;

        private bool m_IsRunning;

        private bool m_IsLoop;

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
            private static readonly List<Timer> s_RunningTimers = new(128);

            private static readonly List<Timer> s_StoppedTimers = new(32);

            private static bool s_IsTaskStarted;

            public static void StartTimer(Timer timer)
            {
                if (timer == null)
                {
                    return;
                }
                int index = s_RunningTimers.Count;
                timer.m_Index = index;
                s_RunningTimers.Add(timer);
                if (index == 0 && !s_IsTaskStarted)
                {
                    Update().Forget();
                    s_IsTaskStarted = true;
                }
            }

            public static void StopTimer(Timer timer)
            {
                if (timer == null)
                {
                    return;
                }
                s_StoppedTimers.Add(timer);
            }

            private static async UniTaskVoid Update()
            {
                while (s_RunningTimers.Count > 0)
                {
                    //进来先停一帧，避免第一个添加的计时器跑一帧，同帧添加的计时器少跑一帧
                    await UniTask.NextFrame();
                    for (int i = 0; i < s_RunningTimers.Count; i++)
                    {
                        s_RunningTimers[i].Update(Time.deltaTime);
                    }
                    int stoppedTimerCount = s_StoppedTimers.Count;
                    if (stoppedTimerCount > 0)
                    {
                        for (int i = 0; i < stoppedTimerCount; i++)
                        {
                            Timer timer = s_StoppedTimers[i];
                            int removeIndex = timer.m_Index;
                            int lastIndex = s_RunningTimers.Count - 1;
                            s_RunningTimers[lastIndex].m_Index = removeIndex;
                            (s_RunningTimers[removeIndex], s_RunningTimers[lastIndex]) = (s_RunningTimers[lastIndex], s_RunningTimers[removeIndex]);
                            s_RunningTimers.RemoveAt(lastIndex);
                        }
                        s_StoppedTimers.Clear();
                    }
                }
                s_IsTaskStarted = false;
            }
        }
    }
}