using System.Collections;
using System.Collections.Generic;

namespace YBFramework.Component
{
    public interface IPortConnectionSource
    {
        private sealed class ConnectedPortDataEnumerator : IEnumerator<ConnectedPortData>, IEnumerable<ConnectedPortData>
        {
            private readonly IPortConnectionSource m_Source;

            private int m_Index;

            public ConnectedPortDataEnumerator(IPortConnectionSource source)
            {
                m_Source = source;
                m_Index = 0;
            }

            public ConnectedPortData Current { get; private set; }

            object IEnumerator.Current => Current;

            public bool MoveNext()
            {
                Current = m_Source.ConnectedPortDataIterator(m_Index++);
                return Current != null;
            }

            public void Reset()
            {
                m_Index = 0;
            }

            public IEnumerator<ConnectedPortData> GetEnumerator()
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
        }

        void Connect(BaseNode node, ConnectedPortData data);

        void Connect(IReadOnlyList<BaseNode> nodes);

        ConnectedPortData ConnectedPortDataIterator(int index);

        IEnumerable<ConnectedPortData> GetConnectedPortDataEnumerator()
        {
            return new ConnectedPortDataEnumerator(this);
        }
#if UNITY_EDITOR
        public static ConnectedPortData FindConnectedPortData(IEnumerable<ConnectedPortData> collection, int nodeID, int portID)
        {
            if (collection != null)
            {
                foreach (ConnectedPortData connectedPortData in collection)
                {
                    if (connectedPortData.NodeID == nodeID && connectedPortData.PortID == portID)
                    {
                        return connectedPortData;
                    }
                }
            }
            return null;
        }

        bool CanConnect(BasePort other);

        void Connect(int nodeID, BasePort other);

        void Disconnect(int nodeID, BasePort other);

        void CheckConnectValid(IReadOnlyList<BaseNode> nodes);
#endif
    }
}