using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

        [SerializeField] protected int m_ID;
        
        public IEnumerable<BasePort> GetPortEnumerable()
        {
            return new PortEnumerator(this);
        }

        public BasePort GetPort(int portID)
        {
            foreach (BasePort port in GetPortEnumerable())
            {
                if (port.GetID() == portID)
                {
                    return port;
                }
            }
            return null;
        }

        public int GetID()
        {
            return m_ID;
        }

        protected abstract BasePort PortIterator(int index);

        public abstract BaseNode Clone();

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
#if UNITY_EDITOR
        private sealed class PortContentCreatorEnumerator : IEnumerable<PortDrawTarget>, IEnumerator<PortDrawTarget>
        {
            private readonly BaseNode m_Node;

            private int m_Index;

            public PortContentCreatorEnumerator(BaseNode node)
            {
                m_Node = node;
                m_Index = 0;
            }

            public PortDrawTarget Current { get; private set; }

            object IEnumerator.Current => Current;

            public IEnumerator<PortDrawTarget> GetEnumerator()
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

        public IEnumerable<PortDrawTarget> GetPortDrawTargetEnumerable()
        {
            return new PortContentCreatorEnumerator(this);
        }

        protected abstract PortDrawTarget PortDrawTargetIterator(int index);

        public abstract void InitNodeInfo();

        public void SetID(int nodeID)
        {
            m_ID = nodeID;
        }
#endif
    }
}