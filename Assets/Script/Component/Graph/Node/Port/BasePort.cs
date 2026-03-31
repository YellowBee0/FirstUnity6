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
    {
#if DEBUG
        [NonSerialized] public BaseNode Node;
#endif
        [HideInInspector] public int ID;

        [HideInInspector] public int ConnectedCount;

        public virtual void Copy(BasePort from)
        {
            ID = from.ID;
        }
#if UNITY_EDITOR
        protected static readonly StringBuilder s_StrBuilder = new();

        public PortViewInfo PortViewInfo;

        public abstract string GetConnectTip();
#endif
    }
}