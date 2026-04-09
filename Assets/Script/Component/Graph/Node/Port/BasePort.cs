using System;
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
        public int ID;

        public virtual void Copy(BasePort from)
        {
            ID = from.ID;
        }
#if UNITY_EDITOR
        protected static readonly StringBuilder s_StrBuilder = new();

        public int ConnectedCount;

        public PortViewInfo PortViewInfo;

        public abstract string GetConnectTip();
#endif
    }
}