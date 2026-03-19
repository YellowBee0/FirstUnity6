using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace YBFramework.Component
{
#if UNITY_EDITOR
    [CreateAssetMenu(fileName = "New BuffData", menuName = "BuffData")]
#endif
    public sealed class BuffData : ScriptableObject
    {
        [SerializeReference] public List<BuffBehaviour> m_Behaviours;
        
        [SerializeReference] private RepeatAddProcess m_RepeatAddProcess;

        [SerializeField] private string m_BuffName;

        [SerializeField] private BuffType m_BuffType;

        public IReadOnlyList<BuffBehaviour> GetBehaviours()
        {
            return m_Behaviours;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string GetBuffName()
        {
            return m_BuffName;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BuffType GetBuffType()
        {
            return m_BuffType;
        }
    }
}