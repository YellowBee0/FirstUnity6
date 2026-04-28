using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
#endif
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

        public override bool CanConnect(BasePort other)
        {
            if (other is MethodPort methodPort)
            {
                if (methodPort.GetParameters().Length == 0)
                {
                    return typeof(TValue).IsAssignableFrom(methodPort.GetReturnType());
                }
            }
            return false;
        }

        public override void Connect(int nodeID, BasePort other)
        {
            MethodPort methodPort = (MethodPort)other;
            Type valueType = typeof(TValue);
            Type returnType = methodPort.GetReturnType();
            bool isExplicitCast = returnType.IsValueType && returnType != valueType;
            m_ConnectedPortData = new ConnectedDelegatePortData
            {
                NodeID = nodeID,
                PortID = other.ID,
                IsExplicitCast = isExplicitCast
            };
            ConnectedCount++;
            other.ConnectedCount++;
        }

        public override void Disconnect(int nodeID, BasePort other)
        {
            if (m_ConnectedPortData != null)
            {
                if (m_ConnectedPortData.NodeID == nodeID && m_ConnectedPortData.PortID == other.ID)
                {
                    m_ConnectedPortData = null;
                    ConnectedCount--;
                    other.ConnectedCount--;
                }
            }
        }

        public override void InitPortViewInfo(string fieldName, string name, Direction direction, Port.Capacity capacity, Color color)
        {
            direction = Direction.Input;
            capacity = Port.Capacity.Single;
            color = Color.blue;
            base.InitPortViewInfo(fieldName, name, direction, capacity, color);
        }

        public override VisualElement CreatePortContentView(SerializedProperty property, out NewPortView portView)
        {
            VisualElement root = base.CreatePortContentView(property, out portView);
            SerializedProperty valueProperty = property.FindPropertyRelative(nameof(m_Value));
            if (valueProperty != null)
            {
                root = new VisualElement();
                PropertyField fieldView = new();
                //TODO:做一个USS资源管理器
                fieldView.styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/USS/PropertyLabel.uss"));
                fieldView.BindProperty(valueProperty);
                root.Add(portView);
                root.Add(fieldView);
            }
            return root;
        }
#endif
    }
}