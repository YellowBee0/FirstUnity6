using System;
using System.Collections.Generic;
using UnityEngine;

namespace YBFramework.Component
{
    public sealed class BuffManager : IComponent
    {
        private readonly List<Buff> m_Buffs = new();

        private Entity m_Owner;

        private float m_BuffMagnification = 1f;

        private float m_DeBuffMagnification = 1f;

        public void SetBuffMagnification(BuffType buffType, float magnification)
        {
            switch (buffType)
            {
                case BuffType.BUFF:
                    if (Mathf.Approximately(m_BuffMagnification, magnification))
                    {
                        return;
                    }
                    m_BuffMagnification = magnification;
                    break;
                case BuffType.DeBUFF:
                    if (Mathf.Approximately(m_DeBuffMagnification, magnification))
                    {
                        return;
                    }
                    m_DeBuffMagnification = magnification;
                    break;
                default:
                    return;
            }
            for (int i = 0; i < m_Buffs.Count; i++)
            {
                if (m_Buffs[i].GetBuffAsset().GetBuffType() == buffType)
                {
                    m_Buffs[i].SetMagnification(magnification);
                }
            }
        }

        public Buff GetBuff(string name)
        {
            for (int i = 0; i < m_Buffs.Count; i++)
            {
                if (m_Buffs[i].GetBuffAsset().GetName() == name)
                {
                    return m_Buffs[i];
                }
            }
            return null;
        }

        public IReadOnlyList<Buff> GetBuffs()
        {
            return m_Buffs;
        }

        public void AddBuff(Entity castor, BuffAsset buffAsset)
        {
            RepeatAddProcess repeatAddProcess = buffAsset.GetRepeatAddProcess();
            Buff existBuff = repeatAddProcess?.CheckIsRepeatAdd(this, castor);
            if (existBuff != null)
            {
                if (!repeatAddProcess.DoRepeatAdd(existBuff))
                {
                    return;
                }
            }
            Buff buff = Buff.Allocate(castor, buffAsset, this);
            switch (buffAsset.GetBuffType())
            {
                case BuffType.BUFF:
                    buff.SetMagnification(m_BuffMagnification);
                    break;
                case BuffType.DeBUFF:
                    buff.SetMagnification(m_DeBuffMagnification);
                    break;
                case BuffType.Neutral:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            m_Buffs.Add(buff);
        }

        private void RemoveBuff(int index)
        {
            Buff.Free(m_Buffs[index]);
            int lastIndex = m_Buffs.Count - 1;
            (m_Buffs[index], m_Buffs[lastIndex]) = (m_Buffs[lastIndex], m_Buffs[index]);
            m_Buffs.RemoveAt(lastIndex);
        }

        public void RemoveBuff(Buff buff)
        {
            for (int i = 0; i < m_Buffs.Count; i++)
            {
                if (m_Buffs[i] == buff)
                {
                    RemoveBuff(i);
                    return;
                }
            }
        }

        public void RemoveBuff(BuffType buffType)
        {
            int i = 0;
            while (i < m_Buffs.Count)
            {
                if (m_Buffs[i].GetBuffAsset().GetBuffType() == buffType)
                {
                    RemoveBuff(i);
                }
                else
                {
                    i++;
                }
            }
        }

        private void Clear()
        {
            for (int i = 0; i < m_Buffs.Count; i++)
            {
                Buff.Free(m_Buffs[i]);
            }
            m_Buffs.Clear();
        }

        public Entity GetOwner()
        {
            return m_Owner;
        }

        public void OnAddComponent(Entity entity)
        {
            m_Owner = entity;
        }

        public void OnRemoveComponent()
        {
            Clear();
        }

        public void ResetComponent()
        {
            Clear();
        }
    }
}