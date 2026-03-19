using System;
using System.Collections.Generic;
using UnityEngine;

namespace YBFramework.Component
{
    [Serializable]
    public abstract class RepeatAddProcess
    {
        protected BuffData m_BuffData;

        [SerializeField]
        private RepeatAddCondition m_RepeatAddCondition;

        [SerializeField]
        private bool m_IsJudgingByCaster;

        public void Init(BuffData buffData)
        {
            m_BuffData = buffData;
        }

        public bool CheckIsRepeatAdd(BuffManager manager, Entity caster, out Buff existBuff)
        {
            existBuff = null;
            IReadOnlyList<Buff> buffs = manager.GetBuffs();
            switch (m_RepeatAddCondition)
            {
                case RepeatAddCondition.SameData:
                    for (int i = 0; i < buffs.Count; i++)
                    {
                        Buff buff = buffs[i];
                        if (buff.GetBUFFData() == m_BuffData)
                        {
                            if (m_IsJudgingByCaster)
                            {
                                if (buff.Caster == caster)
                                {
                                    existBuff = buff;
                                    return true;
                                }
                            }
                        }
                    }
                    break;
                case RepeatAddCondition.SameName:
                    for (int i = 0; i < buffs.Count; i++)
                    {
                        Buff buff = buffs[i];
                        if (buff.GetBUFFData().GetName() == m_BuffData.GetName())
                        {
                            if (m_IsJudgingByCaster)
                            {
                                if (buff.Caster == caster)
                                {
                                    existBuff = buff;
                                    return true;
                                }
                            }
                        }
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return false;
        }

        /// <summary>
        /// 执行重复添加操作
        /// </summary>
        /// <param name="existBuff">已存在的buff</param>
        /// <returns>重复添加操作后是否可以继续添加这个buff</returns>
        public abstract bool DoRepeatAdd(Buff existBuff);
    }
}