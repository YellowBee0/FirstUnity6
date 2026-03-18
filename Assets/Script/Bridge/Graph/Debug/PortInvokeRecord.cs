using System.Collections.Generic;
using YBFramework.Component;

namespace YBFramework.MyEditor
{
    public sealed class PortInvokeRecord
    {
        private static readonly Queue<PortInvokeRecord> s_Pool = new();

        public static PortInvokeRecord Allocate(BasePort fromPort, ConnectedPortData toPortData)
        {
            PortInvokeRecord record = s_Pool.Count > 0 ? s_Pool.Dequeue() : new PortInvokeRecord();
            record.FromNodeID = fromPort.Node.GetID();
            record.FromPortID = fromPort.GetID();
            record.ToNodeID = toPortData.NodeID;
            record.ToPortID = toPortData.PortID;
            return record;
        }

        public static void Free(PortInvokeRecord record)
        {
            record.PreRecord = null;
            record.NextRecord = null;
            s_Pool.Enqueue(record);
        }

        public PortInvokeRecord PreRecord;

        public PortInvokeRecord NextRecord;

        public int FromNodeID;

        public int FromPortID;

        public int ToNodeID;

        public int ToPortID;
    }
}