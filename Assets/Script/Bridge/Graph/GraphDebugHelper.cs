using System.Collections.Generic;
using YBFramework.Component;

namespace YBFramework.MyEditor
{
    public static class GraphDebugHelper
    {
        public sealed class PortInvokeRecord
        {
            private static readonly Queue<PortInvokeRecord> s_Pool = new();

            public static PortInvokeRecord Allocate(BasePort fromPort, BasePort toPort)
            {
                PortInvokeRecord record = s_Pool.Count > 0 ? s_Pool.Dequeue() : new PortInvokeRecord();
                record.FromNodeID = fromPort.Node.GetID();
                record.FromPortID = fromPort.GetID();
                record.ToNodeID = toPort.Node.GetID();
                record.ToPortID = toPort.GetID();
                return record;
            }

            public static void Free(PortInvokeRecord record)
            {
                record.PrePortInvokeRecord = null;
                record.NextPortInvokeRecord = null;
                s_Pool.Enqueue(record);
            }

            public PortInvokeRecord PrePortInvokeRecord;

            public PortInvokeRecord NextPortInvokeRecord;

            public int FromNodeID;

            public int FromPortID;

            public int ToNodeID;

            public int ToPortID;
        }

        private static readonly Dictionary<Graph, PortInvokeRecord> m_GraphRecords = new();

        public static void RecordInvoke(Graph graph, BasePort fromPort, BasePort toPort)
        {
            PortInvokeRecord portInvokeRecord = PortInvokeRecord.Allocate(fromPort, toPort);
            if (m_GraphRecords.TryGetValue(graph, out PortInvokeRecord firstPortInvokeRecord))
            {
                PortInvokeRecord lastRecord = firstPortInvokeRecord;
                while (lastRecord.NextPortInvokeRecord != null)
                {
                    lastRecord = lastRecord.NextPortInvokeRecord;
                }
                lastRecord.NextPortInvokeRecord = portInvokeRecord;
                portInvokeRecord.PrePortInvokeRecord = lastRecord;
                return;
            }
            m_GraphRecords.Add(graph, portInvokeRecord);
        }

        public static PortInvokeRecord GetPortInvokeRecord(Graph graph)
        {
            return m_GraphRecords.GetValueOrDefault(graph);
        }
        
        public static void Clear()
        {
            foreach (KeyValuePair<Graph, PortInvokeRecord> graphRecord in m_GraphRecords)
            {
                PortInvokeRecord portInvokeRecord = graphRecord.Value;
                while (portInvokeRecord != null)
                {
                    PortInvokeRecord.Free(portInvokeRecord);
                    portInvokeRecord = portInvokeRecord.NextPortInvokeRecord;
                }
            }
            m_GraphRecords.Clear();
        }
    }
}