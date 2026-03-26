using System;
using System.Collections.Generic;
using UnityEngine;

namespace YBFramework.Component
{
    [Serializable]
    public abstract class RepeatAddProcess
    {
        private static bool IsSameData(BuffAsset left, BuffAsset right)
        {
            return left == right;
        }

        private static bool IsSameName(BuffAsset left, BuffAsset right)
        {
            return left.GetName() == right.GetName();
        }

        protected BuffAsset m_BuffAsset;

        [SerializeField] private RepeatAddCondition m_RepeatAddCondition;

        [SerializeField] private bool m_IsJudgingByCaster;

        public void Init(BuffAsset buffAsset)
        {
            m_BuffAsset = buffAsset;
        }

        public unsafe Buff CheckIsRepeatAdd(BuffManager manager, Entity caster)
        {
            IReadOnlyList<Buff> buffs = manager.GetBuffs();
            delegate*<BuffAsset, BuffAsset, bool> isMatch;
            switch (m_RepeatAddCondition)
            {
                case RepeatAddCondition.SameData:
                    isMatch = &IsSameData;
                    break;
                case RepeatAddCondition.SameName:
                    isMatch = &IsSameName;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            for (int i = 0; i < buffs.Count; i++)
            {
                Buff buff = buffs[i];
                if (isMatch(buff.GetBuffAsset(), m_BuffAsset) && !m_IsJudgingByCaster || buff.GetCaster() == caster)
                {
                    return buff;
                }
            }
            return null;
        }

        /// <summary>
        ///     执行重复添加操作
        /// </summary>
        /// <param name="existBuff">已存在的buff</param>
        /// <returns>重复添加操作后是否可以继续添加这个buff</returns>
        public abstract bool DoRepeatAdd(Buff existBuff);
    }
}