using System.Collections.Generic;

namespace YBFramework.Component
{
    public sealed class BUFFManager : IComponent
    {
        private readonly List<BUFF> m_BUFFs = new();

        private Entity m_Owner;

        private float m_BUFFMagnification = 1f;

        private float m_DeBUFFMagnification = 1f;

        #region IComponent Members
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
        #endregion

        private void Clear()
        {
            for (var i = 0; i < m_BUFFs.Count; i++) BUFF.Free(m_BUFFs[i]);
            m_BUFFs.Clear();
        }

        public BUFF GetBUFF(string name)
        {
            for (var i = 0; i < m_BUFFs.Count; i++)
            {
                if (m_BUFFs[i].GetBUFFData().GetBUFFName() == name)
                {
                    return m_BUFFs[i];
                }
            }
            return null;
        }

        public void AddBUFF(BUFFData buffData)
        {
            for (var i = 0; i < m_BUFFs.Count; i++)
                if (m_BUFFs[i].GetBUFFData().GetBUFFName() == buffData.GetBUFFName())
                {
                    if (m_BUFFs[i].Stack(buffData)) return;
                    break;
                }
            var buff = BUFF.Allocate<BUFF>();
            buff.Init(buffData, this);
            switch (buffData.GetBUFFType())
            {
                case BUFFType.BUFF:
                    buff.SetMagnification(m_BUFFMagnification);
                    break;
                case BUFFType.DeBUFF:
                    buff.SetMagnification(m_DeBUFFMagnification);
                    break;
            }
            m_BUFFs.Add(buff);
        }

        private void RemoveBUFF(int index)
        {
            BUFF.Free(m_BUFFs[index]);
            (m_BUFFs[index], m_BUFFs[^1]) = (m_BUFFs[^1], m_BUFFs[index]);
            m_BUFFs.RemoveAt(index);
        }

        public void RemoveBUFF(BUFFData buffData)
        {
            for (var i = 0; i < m_BUFFs.Count; i++)
                if (m_BUFFs[i].GetBUFFData().GetBUFFName() == buffData.GetBUFFName())
                {
                    RemoveBUFF(i);
                    return;
                }
        }

        public void RemoveBUFF(BUFF buff)
        {
            for (var i = 0; i < m_BUFFs.Count; i++)
                if (m_BUFFs[i] == buff)
                {
                    RemoveBUFF(i);
                    return;
                }
        }

        public void RemoveBUFF(BUFFType buffType)
        {
            var i = 0;
            while (i < m_BUFFs.Count)
                if (m_BUFFs[i].GetBUFFData().GetBUFFType() == buffType)
                    RemoveBUFF(i);
                else
                    i++;
        }

        public void Start()
        {
            for (var i = 0; i < m_BUFFs.Count; i++) m_BUFFs[i].Start();
        }

        public void Start(BUFFType buffType)
        {
            for (var i = 0; i < m_BUFFs.Count; i++)
                if (m_BUFFs[i].GetBUFFData().GetBUFFType() == buffType)
                    m_BUFFs[i].Start();
        }

        public void Stop()
        {
            for (var i = 0; i < m_BUFFs.Count; i++)
            {
                m_BUFFs[i].Stop();
            }
        }

        public void Stop(BUFFType buffType)
        {
            for (var i = 0; i < m_BUFFs.Count; i++)
            {
                if (m_BUFFs[i].GetBUFFData().GetBUFFType() == buffType)
                {
                    m_BUFFs[i].Stop();
                }
            }
        }

        public void Reset()
        {
            for (var i = 0; i < m_BUFFs.Count; i++)
            {
                m_BUFFs[i].Reset();
            }
        }

        public void Reset(BUFFType buffType)
        {
            for (var i = 0; i < m_BUFFs.Count; i++)
            {
                if (m_BUFFs[i].GetBUFFData().GetBUFFType() == buffType)
                {
                    m_BUFFs[i].Reset();
                }
            }
        }

        public void SetBUFFMagnification(BUFFType buffType, float magnification)
        {
            switch (buffType)
            {
                case BUFFType.BUFF:
                    if (m_BUFFMagnification == magnification)
                    {
                        return;
                    }
                    m_BUFFMagnification = magnification;
                    break;
                case BUFFType.DeBUFF:
                    if (m_DeBUFFMagnification == magnification)
                    {
                        return;
                    }
                    m_DeBUFFMagnification = magnification;
                    break;
                default:
                    return;
            }
            for (var i = 0; i < m_BUFFs.Count; i++)
                if (m_BUFFs[i].GetBUFFData().GetBUFFType() == buffType)
                {
                    m_BUFFs[i].SetMagnification(magnification);
                }
        }
    }
}