using System;
#if UNITY_EDITOR
using System.Text;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
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

        private string m_Name;
        
        private Direction m_Direction;
        
        private Port.Capacity m_Capacity;

        private Color m_Color;

        public string GetName()
        {
            return m_Name;
        }

        public Direction GetDirection()
        {
            return m_Direction;
        }

        public Port.Capacity GetCapacity()
        {
            return m_Capacity;
        }

        public Color GetColor()
        {
            return m_Color;
        }
        
        public abstract string GetConnectTip();

        public virtual VisualElement FillPortView(NewPortView portView)
        {
            throw new NotImplementedException();
        }
#endif
    }
}