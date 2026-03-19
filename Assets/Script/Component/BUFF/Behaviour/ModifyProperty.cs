using System;
using UnityEngine;
using YBFramework.Common;

namespace YBFramework.Component
{
#if UNITY_EDITOR
    [DisplayName("修改属性")]
#endif
    [Serializable]
    public sealed class ModifyProperty : IBuffBehaviour
    {
        private Property m_Property;

        private Buff m_Buff;

        [SerializeReference] private IExecutor m_Executor;

        [SerializeField] private string m_PropertyName;

        [SerializeField] private ValueConstraintType m_ConstraintType;

        [SerializeField] private float m_ModifyValue;

        private void DoModifyValue()
        {
            m_Property?.ModifyValue(m_ConstraintType, m_Buff.GetBUFFData().GetName(), m_ModifyValue);
        }

        public void OnAdd(Buff buff)
        {
            m_Buff = buff;
            m_Property = buff.GetManager().GetOwner().GetCustomComponent<PropertyManager>()?.GetProperty(m_PropertyName);
            m_Executor.Start();
        }

        public void OnRemove()
        {
            m_Executor.Stop();
        }

        public void OnReset()
        {
            m_Executor.Reset();
        }

        public void OnMagnificationChanged()
        {
            m_ModifyValue *= m_Buff.GetMagnification();
        }

        public IBuffBehaviour Clone()
        {
            ModifyProperty modifyProperty = IBuffBehaviour.Allocate<ModifyProperty>();
            modifyProperty.m_PropertyName = m_PropertyName;
            modifyProperty.m_ConstraintType = m_ConstraintType;
            modifyProperty.m_ModifyValue = m_ModifyValue;
            modifyProperty.m_Executor = m_Executor.Clone();
            modifyProperty.m_Executor.SetExecuteAction(DoModifyValue);
            return modifyProperty;
        }
    }
}