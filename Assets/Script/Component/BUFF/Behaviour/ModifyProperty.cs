using System;
using UnityEngine;
using YBFramework.Common;

namespace YBFramework.Component
{
#if UNITY_EDITOR
    [DisplayName("修改属性")]
#endif
    [Serializable]
    public sealed class ModifyProperty : IBuffComponent
    {
        private Property m_Property;

        private Buff m_Buff;

        [SerializeReference] private IExecutor m_Executor;

        [SerializeField] private string m_PropertyName;

        [SerializeField] private ValueConstraintType m_ConstraintType;

        [SerializeField] private float m_ModifyValue;

        private void DoModifyValue()
        {
            m_Property.ModifyValue(m_ConstraintType, m_Buff.GetBuffAsset().GetName(), m_ModifyValue);
        }

        public void OnAdd(Buff buff)
        {
            m_Buff = buff;
            Entity owner = buff.GetManager().GetOwner();
            PropertyManager propertyManager = owner.GetComponent<PropertyManager>();
            if (propertyManager != null)
            {
                m_Property = propertyManager.GetProperty(m_PropertyName);
                if (m_Property != null)
                {
                    m_Executor.Initialize(owner);
                    m_Executor.RegisterExecuteCallback(DoModifyValue);
                    m_Executor.Start();
                }
            }
        }

        public void OnRemove()
        {
            m_Executor.Stop();
            m_Executor.UnregisterExecuteCallback(DoModifyValue);
        }

        public void OnReset()
        {
            m_Executor.Reset();
        }

        public void OnMagnificationChanged()
        {
            m_ModifyValue *= m_Buff.GetMagnification();
        }

        public IBuffComponent Clone()
        {
            ModifyProperty modifyProperty = IBuffComponent.Allocate<ModifyProperty>();
            modifyProperty.m_PropertyName = m_PropertyName;
            modifyProperty.m_ConstraintType = m_ConstraintType;
            modifyProperty.m_ModifyValue = m_ModifyValue;
            modifyProperty.m_Executor ??= m_Executor.Clone();
            return modifyProperty;
        }
    }
}