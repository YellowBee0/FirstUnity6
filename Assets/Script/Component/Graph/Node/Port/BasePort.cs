using System;
using UnityEngine;
#if UNITY_EDITOR
using System.Text;
using YBFramework.MyEditor;
#endif

namespace YBFramework.Component
{
    [Serializable]
    public abstract class BasePort
#if UNITY_EDITOR
        : PortDrawTarget
#endif
    {
#if DEBUG
        [NonSerialized] public BaseNode Node;
#endif
        [SerializeField] private int m_ID;

        public int GetID()
        {
            return m_ID;
        }

        public virtual void Copy(BasePort from)
        {
            m_ID = from.m_ID;
        }
#if UNITY_EDITOR
        protected static readonly StringBuilder s_StrBuilder = new();

        public void SetID(int portID)
        {
            m_ID = portID;
        }

        public abstract string GetConnectTip();
#endif
    }
}