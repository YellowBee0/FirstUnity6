using System;
using System.Reflection;
using UnityEngine;
#if DEBUG
using YBFramework.MyEditor;
#endif

namespace YBFramework.Component
{
    [Serializable]
    public sealed class FuncPort<TValue> : DelegatePort<Func<TValue>>
    {
        [SerializeField] private ConnectedDelegatePortData m_ConnectedPortData;

        [SerializeField] private TValue m_Value;

        public TValue Invoke()
        {
#if DEBUG
            GraphDebugHelper.RecordInvoke(Node.Graph, this, m_ConnectedPortData);
#endif
            return m_Delegate != null ? m_Delegate.Invoke() : m_Value;
        }

        public override void Copy(BasePort from)
        {
            base.Copy(from);
            if (from is FuncPort<TValue> funcPort)
            {
                //TODO:这里直接=号赋值，对于引用类型就是指向同一个实例，可能有时候想要不同的实例
                m_ConnectedPortData = funcPort.m_ConnectedPortData;
                m_Value = funcPort.m_Value;
            }
        }

        public override object DynamicInvoke(params object[] args)
        {
            return Invoke();
        }

        public override ConnectedPortData ConnectedPortDataIterator(int index)
        {
            return index == 0 ? m_ConnectedPortData.NodeID == 0 && m_ConnectedPortData.PortID == 0 ? null : m_ConnectedPortData : null;
        }
#if UNITY_EDITOR
        public TValue GetValue()
        {
            return m_Value;
        }

        public void SetValue(TValue value)
        {
            m_Value = value;
        }

        public override string GetConnectTip()
        {
            return $"no parameter return {typeof(TValue)}";
        }

        protected override bool CanConnect(BasePort other, out bool isExplicitCast)
        {
            isExplicitCast = false;
            if (other is MethodPort methodPort)
            {
                ParameterInfo[] methodPortParameterInfos = methodPort.GetParameters();
                if (methodPortParameterInfos.Length == 0)
                {
                    Type methodPortReturnType = methodPort.GetReturnType();
                    Type valueType = typeof(TValue);
                    if (methodPortReturnType.IsValueType && methodPortReturnType != valueType)
                    {
                        isExplicitCast = true;
                    }
                    return valueType.IsAssignableFrom(methodPortReturnType);
                }
            }
            return false;
        }

        public override bool Connect(int nodeID, BasePort other)
        {
            if (CanConnect(other, out bool isExplicitCast))
            {
                if (m_ConnectedPortData != null)
                {
                    if (m_ConnectedPortData.NodeID == nodeID && m_ConnectedPortData.PortID == other.ID)
                    {
                        return false;
                    }
                }
                else
                {
                    m_ConnectedPortData = new ConnectedDelegatePortData();
                }
                m_ConnectedPortData.NodeID = nodeID;
                m_ConnectedPortData.PortID = other.ID;
                m_ConnectedPortData.IsExplicitCast = isExplicitCast;
                ConnectedCount++;
                other.ConnectedCount++;
                return true;
            }
            return false;
        }

        public override bool Disconnect(int nodeID, BasePort other)
        {
            if (m_ConnectedPortData != null)
            {
                if (m_ConnectedPortData.NodeID == nodeID && m_ConnectedPortData.PortID == other.ID)
                {
                    m_ConnectedPortData = null;
                    ConnectedCount--;
                    other.ConnectedCount--;
                    return true;
                }
            }
            return false;
        }
#endif
    }
}