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
        [SerializeReference] public List<IBuffBehaviour> m_Behaviours;
        
        [SerializeReference] private RepeatAddProcess m_RepeatAddProcess;

        [SerializeField] private string m_BuffName;

        [SerializeField] private BuffType m_BuffType;

        public IReadOnlyList<IBuffBehaviour> GetBehaviours()
        {
            return m_Behaviours;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string GetName()
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