using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace YBFramework.Component
{
    public sealed class CustomAnimation
    {
        private static readonly Queue<CustomAnimation> s_Pool = new();

        /// <summary>
        /// 动画持有者，主要用于动画事件注册
        /// </summary>
        private object m_Owner;

        private string m_BindPortName;

        private AnimationClip m_AnimationClip;
        
        private AnimationClipPlayable m_AnimationClipPlayable;

        private float m_Lenght;

        private bool m_IsLoopAnimation;
        
        private bool m_IsPlaying;

        public void Play()
        {
            if (m_IsPlaying)
            {
                return;
            }
            m_AnimationClipPlayable.Play();
            m_IsPlaying = true;
        }
        
        public void Pause()
        {
            if (!m_IsPlaying)
            {
                return;
            }
            m_AnimationClipPlayable.Pause();
            m_IsPlaying = false;
        }
    }
}