using System.Collections.Generic;
using YBFramework.Component;

namespace YBFramework.MyEditor
{
    public static class GraphDebugHelper
    {
        private static readonly HashSet<Graph> s_RunningGraphs = new();

        private static readonly Dictionary<Graph, PortInvokeRecord> m_GraphRecords = new();

        public static IReadOnlyCollection<Graph> GetRunningGraphs()
        {
            return s_RunningGraphs;
        }

        public static void AddRunningGraph(Graph graph)
        {
            s_RunningGraphs.Add(graph);
        }

        public static void RemoveRunningGraph(Graph graph)
        {
            s_RunningGraphs.Remove(graph);
        }

        public static void RecordInvoke(Graph graph, BasePort fromPort, ConnectedPortData toPortData)
        {
            if (toPortData != null)
            {
                PortInvokeRecord newRecord = PortInvokeRecord.Allocate(fromPort, toPortData);
                if (m_GraphRecords.TryGetValue(graph, out PortInvokeRecord firstRecord))
                {
                    PortInvokeRecord latestRecord = firstRecord;
                    while (latestRecord.NextRecord != null)
                    {
                        latestRecord = latestRecord.NextRecord;
                    }
                    latestRecord.NextRecord = newRecord;
                    newRecord.PreRecord = latestRecord;
                    return;
                }
                m_GraphRecords.Add(graph, newRecord);
            }
        }

        public static void RecordInvoke(Graph graph, BasePort fromPort, IEnumerable<ConnectedPortData> toPortData)
        {
            PortInvokeRecord headRecord = null;
            PortInvokeRecord tailRecord = null;
            foreach (ConnectedPortData connectedPortData in toPortData)
            {
                PortInvokeRecord newRecord = PortInvokeRecord.Allocate(fromPort, connectedPortData);
                if (headRecord == null)
                {
                    headRecord = newRecord;
                }
                else
                {
                    tailRecord.NextRecord = newRecord;
                    newRecord.PreRecord = tailRecord;
                }
                tailRecord = newRecord;
            }
            if (headRecord != null)
            {
                if (m_GraphRecords.TryGetValue(graph, out PortInvokeRecord firstRecord))
                {
                    PortInvokeRecord lastRecord = firstRecord;
                    while (lastRecord.NextRecord != null)
                    {
                        lastRecord = lastRecord.NextRecord;
                    }
                    lastRecord.NextRecord = headRecord;
                    headRecord.PreRecord = lastRecord;
                    return;
                }
                m_GraphRecords.Add(graph, headRecord);
            }
        }

        public static void Clear()
        {
            foreach (KeyValuePair<Graph, PortInvokeRecord> graphRecord in m_GraphRecords)
            {
                PortInvokeRecord portInvokeRecord = graphRecord.Value;
                while (portInvokeRecord != null)
                {
                    PortInvokeRecord.Free(portInvokeRecord);
                    portInvokeRecord = portInvokeRecord.NextRecord;
                }
            }
            m_GraphRecords.Clear();
        }
    }
}