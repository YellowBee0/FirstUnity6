using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using YBFramework.MyEditor;

namespace YBFramework.Component
{
    [Serializable]
    public abstract class BaseNode
    {
        protected static void CopyPort(BaseNode from, BaseNode to)
        {
            int index = 0;
            BasePort port;
            while ((port = to.PortIterator(index)) != null)
            {
                port.Copy(from.PortIterator(index));
                index++;
            }
        }

        private sealed class PortEnumerator : IEnumerable<BasePort>, IEnumerator<BasePort>
        {
            private readonly BaseNode m_Node;

            private int m_Index;

            public PortEnumerator(BaseNode node)
            {
                m_Node = node;
                m_Index = 0;
            }

            public BasePort Current { get; private set; }

            object IEnumerator.Current => Current;

            public IEnumerator<BasePort> GetEnumerator()
            {
                return this;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                Current = m_Node.PortIterator(m_Index++);
                return Current != null;
            }

            public void Reset()
            {
                m_Index = 0;
            }
        }
#if DEBUG
        [NonSerialized] public Graph Graph;
#endif
        public int ID;

        public IEnumerable<BasePort> GetPortEnumerable()
        {
            return new PortEnumerator(this);
        }

        public BasePort GetPort(int portID)
        {
            foreach (BasePort port in GetPortEnumerable())
            {
                if (port.ID == portID)
                {
                    return port;
                }
            }
            return null;
        }

        protected abstract BasePort PortIterator(int index);

        public abstract BaseNode Clone();
#if UNITY_EDITOR
        private sealed class PortContentCreatorEnumerator : IEnumerable<BasePort>, IEnumerator<BasePort>
        {
            private readonly BaseNode m_Node;

            private int m_Index;

            public PortContentCreatorEnumerator(BaseNode node)
            {
                m_Node = node;
                m_Index = 0;
            }

            public BasePort Current { get; private set; }

            object IEnumerator.Current => Current;

            public IEnumerator<BasePort> GetEnumerator()
            {
                return this;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                Current = m_Node.PortDrawTargetIterator(m_Index++);
                return Current != null;
            }

            public void Reset()
            {
                m_Index = 0;
            }
        }

        public IEnumerable<BasePort> GetPortDrawTargetEnumerable()
        {
            return new PortContentCreatorEnumerator(this);
        }

        protected virtual BasePort PortDrawTargetIterator(int index)
        {
            return PortIterator(index);
        }

        public abstract void InitNodeInfo();

        public virtual void FillNodeView(NewNodeView nodeView)
        {
            foreach (BasePort port in GetPortEnumerable())
            {
                Direction direction = port.GetDirection();
                NewPortView portView = new(port, port.GetName(), direction, port.GetCapacity(), port.GetColor());
                VisualElement portContainer = port.FillPortView(portView);
                if (direction == Direction.Output)
                {
                    nodeView.inputContainer.Add(portContainer);
                }
                else
                {
                    nodeView.outputContainer.Add(portContainer);
                }
            }
        }
#endif
    }
}