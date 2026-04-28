using System;
#if UNITY_EDITOR
using System.Text;
using UnityEditor;
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
        public int ConnectedCount;

        protected static readonly StringBuilder s_StrBuilder = new();

        protected string m_Name;

        protected string m_FieldName;

        protected Direction m_Direction;

        protected Port.Capacity m_Capacity;

        protected Color m_Color;

        public string GetName()
        {
            return m_Name;
        }

        public string GetFieldName()
        {
            return m_FieldName;
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

        public virtual void InitPortViewInfo(string fieldName, string name, Direction direction, Port.Capacity capacity, Color color)
        {
            m_FieldName = fieldName;
            m_Name = name;
            m_Direction = direction;
            m_Capacity = capacity;
            m_Color = color;
        }

        public virtual VisualElement CreatePortContentView(SerializedProperty property, out NewPortView portView)
        {
            portView = new NewPortView(this, m_Name, m_Direction, m_Capacity, m_Color);
            return portView;
        }

        public abstract string GetConnectTip();
#endif
    }
}