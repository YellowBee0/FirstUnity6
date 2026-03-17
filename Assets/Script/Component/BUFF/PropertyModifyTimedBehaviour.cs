using System.ComponentModel;
using UnityEngine;
using YBFramework.Common;

namespace YBFramework.Component
{
#if UNITY_EDITOR
    [DisplayName("间隔一段时间时间修改属性")]
#endif
    public sealed class PropertyModifyTimedBehaviour : TimedBehaviour
    {
        [SerializeField] private ValueConstraintType m_ConstraintType;

        [SerializeField] private float m_ModifiedValue;

        [SerializeField] private string m_PropertyName;

        private Property m_Property;

        private void ModifyValue()
        {
            m_Property.ModifyValue(m_ConstraintType, m_BUFF.GetBUFFData().GetBUFFName(), m_ModifiedValue);
        }

        public override BUFFBehaviour Clone()
        {
            return Allocate<PropertyModifyTimedBehaviour>();
        }

        public override void OnInit(BUFFBehaviour source)
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

        public override void OnAdd(BUFF buff)
        {
            base.OnAdd(buff);
            m_Property = buff.GetManager().GetOwner().GetCustomComponent<PropertyManager>()?.GetProperty(m_PropertyName);
        }
    }
}