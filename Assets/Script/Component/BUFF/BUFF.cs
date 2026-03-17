using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using YBFramework.Common;

namespace YBFramework.Component
{
    public sealed class BUFF : BaseValue<int>
    {
        private readonly List<BUFFBehaviour> m_Behaviours = new();

        private BUFFData m_BUFFData;

        private bool m_IsRunning;

        private float m_Magnification;

        private BUFFManager m_Manager;

        private Action m_OnLayerChanged;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BUFFManager GetManager()
        {
            return m_Manager;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BUFFData GetBUFFData()
        {
            return m_BUFFData;
        }

        public override void ModifyMaxValue(string modifier, int delta)
        {
            if (delta == 0) return;
            int oldValue = m_MaxValue;
            int newValue = m_MaxValue + delta;
            if (newValue < m_MinValue) newValue = m_MinValue;
            m_MaxValue = newValue;
            Record(ValueConstraintType.Max, modifier, delta, newValue - oldValue);
            //TODO 字符串本地化
            if (m_CurValue > m_MaxValue)
            {
                ModifyCurValue(modifier + "(self max value)", m_MaxValue - m_CurValue);
            }
        }

        public override void ModifyMinValue(string modifier, int delta)
        {
            if (delta == 0) return;
            int oldValue = m_MaxValue;
            int newValue = m_MaxValue + delta;
            if (newValue > m_MaxValue) newValue = m_MaxValue;
            m_MinValue = newValue;
            Record(ValueConstraintType.Min, modifier, delta, newValue - oldValue);
            //TODO 字符串本地化
            if (m_CurValue < m_MinValue)
            {
                ModifyCurValue(modifier + "(self min value)", m_MinValue - m_CurValue);
            }
        }

        public override void ModifyCurValue(string modifier, int delta)
        {
            if (delta == 0) return;
            int oldValue = m_CurValue;
            int newValue = m_CurValue + delta;
            if (newValue > m_MaxValue)
            {
                if (m_BUFFData.GetIsRemoveOnMaxLayer())
                {
                    m_Manager.RemoveBUFF(this);
                    return;
                }
                newValue = m_MaxValue;
            }
            else if (newValue < m_MinValue)
            {
                if (m_BUFFData.GetIsRemoveOnMinLayer())
                {
                    m_Manager.RemoveBUFF(this);
                    return;
                }
                newValue = m_MinValue;
            }
            m_CurValue = newValue;
            m_OnLayerChanged?.Invoke();
            Record(ValueConstraintType.Current, modifier, delta, oldValue - newValue);
        }

        protected override void OnFree()
        {
            base.OnFree();
            Stop();
            for (int i = 0; i < m_Behaviours.Count; i++)
            {
                m_Behaviours[i].OnRemove();
            }
            m_Behaviours.Clear();
            m_OnLayerChanged = null;
            m_Magnification = 1;
        }

        public void RegisterLayerChangeCallBack(Action callBack)
        {
            m_OnLayerChanged += callBack;
        }

        public void UnregisterLayerChangeCallBack(Action callBack)
        {
            m_OnLayerChanged -= callBack;
        }

        public void Init(BUFFData buffData, BUFFManager manager)
        {
            m_Manager = manager;
            m_BUFFData = buffData;
            if (buffData.GetStackOption() != StackOption.NotAllow)
            {
                Init(buffData.GetMaxLayer(), buffData.GetMinLayer(), buffData.GetInitialLayer(), false, false, buffData.GetIsRecordLayer());
            }
            else
            {
                Init(0, 0, 0, false, false, false);
            }
            IReadOnlyList<BUFFBehaviour> behaviours = buffData.GetBehaviours();
            for (int i = 0; i < behaviours.Count; i++)
            {
                BUFFBehaviour behaviour = behaviours[i].Clone();
                behaviour.OnInit(behaviours[i]);
                AddBehaviour(behaviour);
            }
        }

        public bool Stack(BUFFData other)
        {
            switch (m_BUFFData.GetStackOption())
            {
                case StackOption.AddLayer:
                    ModifyCurValue("Stack", other.GetInitialLayer());
                    return true;
                case StackOption.Replace:
                    bool isRunning = m_IsRunning;
                    OnFree();
                    Init(other, m_Manager);
                    if (isRunning) Start();
                    return true;
                case StackOption.Reset:
                    Reset();
                    return true;
                default:
                    return false;
            }
        }

        public void AddBehaviour(BUFFBehaviour behaviour)
        {
            m_Behaviours.Add(behaviour);
            behaviour.OnAdd(this);
            if (m_IsRunning)
            {
                behaviour.OnStart();
            }
        }

        public void RemoveBehaviour(BUFFBehaviour behaviour)
        {
            if (m_Behaviours.Remove(behaviour))
            {
                if (m_IsRunning)
                {
                    behaviour.OnStop();
                }
                behaviour.OnRemove();
            }
        }

        public void Start()
        {
            for (int i = 0; i < m_Behaviours.Count; i++)
            {
                m_Behaviours[i].OnStart();
            }
            m_IsRunning = true;
        }

        public void Stop()
        {
            for (int i = 0; i < m_Behaviours.Count; i++)
            {
                m_Behaviours[i].OnStop();
            }
            m_IsRunning = false;
        }

        public void Reset()
        {
            for (int i = 0; i < m_Behaviours.Count; i++)
            {
                m_Behaviours[i].OnReset();
            }
            ModifyCurValue("Reset", m_MinValue - m_MaxValue);
        }

        public bool IsRunning()
        {
            return m_IsRunning;
        }

        public void SetMagnification(float magnification)
        {
            if (m_Magnification != magnification)
            {
                m_Magnification = magnification;
                for (int i = 0; i < m_Behaviours.Count; i++)
                {
                    m_Behaviours[i].OnMagnificationChanged();
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float GetMagnification()
        {
            return m_Magnification;
        }
    }
}