using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace YBFramework.Component
{
#if UNITY_EDITOR
    [CreateAssetMenu(fileName = "NewBuffAsset", menuName = "CreateBuffAsset")]
#endif
    public sealed class BuffAsset : ScriptableObject
    {
        [SerializeField] private string m_BuffName;

        [SerializeField] private BuffType m_BuffType;

        [SerializeReference] private RepeatAddProcess m_RepeatAddProcess;

        [SerializeReference] public List<IBuffComponent> m_Components;

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RepeatAddProcess GetRepeatAddProcess()
        {
            return m_RepeatAddProcess;
        }

        public IReadOnlyList<IBuffComponent> GetComponents()
        {
            return m_Components;
        }
    }
}