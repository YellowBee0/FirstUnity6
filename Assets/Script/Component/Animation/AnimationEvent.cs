using System;
using System.Runtime.CompilerServices;

namespace YBFramework.Component
{
    public abstract class AnimationEvent
    {
        private readonly IAnimationEventSource m_EventSource;

        private readonly AnimationEventData m_EventData;

        public AnimationEvent(IAnimationEventSource eventSource, AnimationEventData eventData)
        {
            m_EventSource = eventSource;
            m_EventData = eventData;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IAnimationEventSource GetEventSource()
        {
            return m_EventSource;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public AnimationEventData GetEventData()
        {
            return m_EventData;
        }

        public abstract void Invoke();

        public abstract void SetArg(AnimationEventArgs arg);
    }

    public sealed class AnimationEvent<T> : AnimationEvent where T : AnimationEventArgs
    {
        /// <summary>
        ///     这个委托只能单播，因为需要比较函数实例对象，如果是全局函数，就直接使用null
        /// </summary>
        private readonly Action<T> m_Action;

        private T m_Arg;

        public AnimationEvent(IAnimationEventSource eventSource, AnimationEventData eventData, Action<T> action) : base(eventSource, eventData)
        {
            m_Action = action;
            SetArg(eventData.EventArgs);
        }

        public override void Invoke()
        {
            m_Action.Invoke(m_Arg);
        }

        public override void SetArg(AnimationEventArgs arg)
        {
            m_Arg = (T)arg;
        }
    }
}