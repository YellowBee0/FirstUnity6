using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace YBFramework.Component
{
    [CreateAssetMenu(fileName = "AnimationAsset", menuName = "YBFramework/Animation/AnimationAsset")]
    public sealed class AnimationAsset : ScriptableObject
    {
        [SerializeField] private AnimationClip m_AnimationClip;

        [SerializeField] private AnimationEventData[] m_AnimationEventData;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public AnimationClip GetAnimationClip()
        {
            return m_AnimationClip;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<AnimationEventData> GetAnimationEventData()
        {
            return m_AnimationEventData;
        }

#if UNITY_EDITOR
        public AnimationEventData[] GetAnimationEventDataList()
        {
            return m_AnimationEventData;
        }
#endif
    }
}