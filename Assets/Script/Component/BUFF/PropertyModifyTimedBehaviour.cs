using System;
using UnityEngine;
using YBFramework.Common;

namespace YBFramework.Component
{
    [Serializable]
    public sealed class PropertyModifyTimedBehaviour : TimedBehaviour
    {
        [SerializeField] private ValueConstraintType m_ConstraintType;

        [SerializeField] private float m_ModifiedValue;

        [SerializeField] private string m_PropertyName;

        private Property m_Property;

        private void ModifyValue()
        {
            m_Property.ModifyValue(m_ConstraintType, m_BUFF.GetBUFFData().GetBuffName(), m_ModifiedValue);
        }

        public override BuffBehaviour Clone()
        {
            return Allocate<PropertyModifyTimedBehaviour>();
        }

        public override void OnInit(BuffBehaviour source)
        {
            if (source is PropertyModifyTimedBehaviour behaviour)
            {
                m_Interval = behaviour.m_Interval;
                m_PropertyName = behaviour.m_PropertyName;
                m_ModifiedValue = behaviour.m_ModifiedValue;
                if (m_Timer == null)
                {
                    m_Timer = Timer.Allocate();
                    m_Timer.SetCallback(ModifyValue);
                }
                m_Timer.SetTotalTime(m_Interval);
                m_Timer.SetIsLoop(true);
            }
        }

        public override void OnMagnificationChanged()
        {
            base.OnMagnificationChanged();
            m_ModifiedValue *= m_BUFF.GetMagnification();
        }

        public override void OnAdd(Buff buff)
        {
            base.OnAdd(buff);
            m_Property = buff.GetManager().GetOwner().GetCustomComponent<PropertyManager>()?.GetProperty(m_PropertyName);
        }
    }

    [Serializable]
    public sealed class TestBehaviour : TimedBehaviour
    {
        public int ID;
        
        public override BuffBehaviour Clone()
        {
            throw new NotImplementedException();
        }

        public override void OnInit(BuffBehaviour source)
        {
            throw new NotImplementedException();
        }
    }
}