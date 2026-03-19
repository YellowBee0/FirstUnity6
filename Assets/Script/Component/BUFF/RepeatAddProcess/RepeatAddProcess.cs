using System;
using System.Collections.Generic;
using UnityEngine;

namespace YBFramework.Component
{
    [Serializable]
    public abstract class RepeatAddProcess
    {
        protected BuffAsset BuffAsset;

        [SerializeField] private RepeatAddCondition m_RepeatAddCondition;

        [SerializeField] private bool m_IsJudgingByCaster;

        public void Init(BuffAsset buffAsset)
        {
            BuffAsset = buffAsset;
        }

        public Buff CheckIsRepeatAdd(BuffManager manager, Entity caster)
        {
            IReadOnlyList<Buff> buffs = manager.GetBuffs();
            switch (m_RepeatAddCondition)
            {
                case RepeatAddCondition.SameData:
                    for (int i = 0; i < buffs.Count; i++)
                    {
                        Buff buff = buffs[i];
                        if (buff.GetBuffAsset() == BuffAsset)
                        {
                            if (m_IsJudgingByCaster)
                            {
                                if (buff.GetCaster() == caster)
                                {
                                    return buff;
                                }
                            }
                        }
                    }
                    break;
                case RepeatAddCondition.SameName:
                    for (int i = 0; i < buffs.Count; i++)
                    {
                        Buff buff = buffs[i];
                        if (buff.GetBuffAsset().GetName() == BuffAsset.GetName())
                        {
                            if (m_IsJudgingByCaster)
                            {
                                if (buff.GetCaster() == caster)
                                {
                                    return buff;
                                }
                            }
                        }
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
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