using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace YBFramework.Component
{
    public sealed class AnimationAsset : ScriptableObject
    {
        [SerializeField] private AnimationClip m_AnimationClip;

        [SerializeField] private AnimationEventData[] m_AnimationEvents;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public AnimationClip GetAnimationClip()
        {
            return m_AnimationClip;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<AnimationEventData> GetAnimationEventData()
        {
            return m_AnimationEvents;
        }
    }
}