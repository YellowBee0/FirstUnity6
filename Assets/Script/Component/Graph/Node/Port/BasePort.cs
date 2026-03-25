using System;
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
        public int ID;

        public int ConnectedCount;

        public virtual void Copy(BasePort from)
        {
            ID = from.ID;
        }
#if UNITY_EDITOR
        protected static readonly StringBuilder s_StrBuilder = new();

        public abstract string GetConnectTip();
#endif
    }
}