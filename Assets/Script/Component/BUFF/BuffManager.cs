using System.Collections.Generic;

namespace YBFramework.Component
{
    public sealed class BuffManager : IComponent
    {
        private readonly List<Buff> m_BUFFs = new();

        private Entity m_Owner;

        private float m_BUFFMagnification = 1f;

        private float m_DeBUFFMagnification = 1f;

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

        private void Clear()
        {
            for (int i = 0; i < m_BUFFs.Count; i++)
            {
                Buff.Free(m_BUFFs[i]);
            }
            m_BUFFs.Clear();
        }

        public IReadOnlyList<Buff> GetBuffs()
        {
            return m_BUFFs;
        }
        
        public Buff GetBUFF(string name)
        {
            for (int i = 0; i < m_BUFFs.Count; i++)
            {
                if (m_BUFFs[i].GetBUFFData().GetBuffName() == name)
                {
                    return m_BUFFs[i];
                }
            }
            return null;
        }

        public void AddBUFF(BuffData buffData)
        {
            for (int i = 0; i < m_BUFFs.Count; i++)
            {
                if (m_BUFFs[i].GetBUFFData().GetBuffName() == buffData.GetBuffName())
                {
                    if (m_BUFFs[i].Stack(buffData))
                    {
                        return;
                    }
                    break;
                }
            }
            Buff buff = Buff.Allocate<Buff>();
            buff.Init(buffData, this);
            switch (buffData.GetBuffType())
            {
                case BuffType.BUFF:
                    buff.SetMagnification(m_BUFFMagnification);
                    break;
                case BuffType.DeBUFF:
                    buff.SetMagnification(m_DeBUFFMagnification);
                    break;
            }
            m_BUFFs.Add(buff);
        }

        private void RemoveBUFF(int index)
        {
            Buff.Free(m_BUFFs[index]);
            (m_BUFFs[index], m_BUFFs[^1]) = (m_BUFFs[^1], m_BUFFs[index]);
            m_BUFFs.RemoveAt(index);
        }

        public void RemoveBUFF(BuffData buffData)
        {
            for (int i = 0; i < m_BUFFs.Count; i++)
            {
                if (m_BUFFs[i].GetBUFFData().GetBuffName() == buffData.GetBuffName())
                {
                    RemoveBUFF(i);
                    return;
                }
            }
        }

        public void RemoveBUFF(Buff buff)
        {
            for (int i = 0; i < m_BUFFs.Count; i++)
            {
                if (m_BUFFs[i] == buff)
                {
                    RemoveBUFF(i);
                    return;
                }
            }
        }

        public void RemoveBUFF(BuffType buffType)
        {
            int i = 0;
            while (i < m_BUFFs.Count)
            {
                if (m_BUFFs[i].GetBUFFData().GetBuffType() == buffType)
                {
                    RemoveBUFF(i);
                }
                else
                {
                    i++;
                }
            }
        }

        public void Start()
        {
            for (int i = 0; i < m_BUFFs.Count; i++)
            {
                m_BUFFs[i].Start();
            }
        }

        public void Start(BuffType buffType)
        {
            for (int i = 0; i < m_BUFFs.Count; i++)
            {
                if (m_BUFFs[i].GetBUFFData().GetBuffType() == buffType)
                {
                    m_BUFFs[i].Start();
                }
            }
        }

        public void Stop()
        {
            for (int i = 0; i < m_BUFFs.Count; i++)
            {
                m_BUFFs[i].Stop();
            }
        }

        public void Stop(BuffType buffType)
        {
            for (int i = 0; i < m_BUFFs.Count; i++)
            {
                if (m_BUFFs[i].GetBUFFData().GetBuffType() == buffType)
                {
                    m_BUFFs[i].Stop();
                }
            }
        }

        public void Reset()
        {
            for (int i = 0; i < m_BUFFs.Count; i++)
            {
                m_BUFFs[i].Reset();
            }
        }

        public void Reset(BuffType buffType)
        {
            for (int i = 0; i < m_BUFFs.Count; i++)
            {
                if (m_BUFFs[i].GetBUFFData().GetBuffType() == buffType)
                {
                    m_BUFFs[i].Reset();
                }
            }
        }

        public void SetBUFFMagnification(BuffType buffType, float magnification)
        {
            switch (buffType)
            {
                case BuffType.BUFF:
                    if (m_BUFFMagnification == magnification)
                    {
                        return;
                    }
                    m_BUFFMagnification = magnification;
                    break;
                case BuffType.DeBUFF:
                    if (m_DeBUFFMagnification == magnification)
                    {
                        return;
                    }
                    m_DeBUFFMagnification = magnification;
                    break;
                default:
                    return;
            }
            for (int i = 0; i < m_BUFFs.Count; i++)
            {
                if (m_BUFFs[i].GetBUFFData().GetBuffType() == buffType)
                {
                    m_BUFFs[i].SetMagnification(magnification);
                }
            }
        }
    }
}