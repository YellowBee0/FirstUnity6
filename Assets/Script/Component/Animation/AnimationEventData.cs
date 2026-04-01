using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace YBFramework.Component
{
    [Serializable]
    public sealed class AnimationEventData
    {
        [SerializeField] private string m_EventName;

        [SerializeField] private float m_TriggerTime;

        /// <summary>
        /// 函数的参数集合，调用函数时只能读不能写入
        /// TODO：需要支持覆盖，不管编辑器中还是运行时
        /// </summary>
        [SerializeReference] private object[] m_Parameters;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string GetEventName()
        {
            return m_EventName;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float GetTriggerTime()
        {
            return m_TriggerTime;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object[] GetParameters()
        {
            return m_Parameters;
        }
    }
}