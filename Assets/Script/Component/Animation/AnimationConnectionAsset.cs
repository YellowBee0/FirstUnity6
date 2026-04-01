using System.Collections.Generic;
using UnityEngine;

namespace YBFramework.Component
{
    public sealed class AnimationConnectionAsset : ScriptableObject
    {
        [SerializeField] private AnimationConnectionData[] m_AnimationConnectionData;

        public IEnumerable<AnimationConnectionData> GetAnimationConnectionData()
        {
            return m_AnimationConnectionData;
        }
    }
}