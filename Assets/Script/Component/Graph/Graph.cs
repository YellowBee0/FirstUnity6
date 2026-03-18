using System.Collections.Generic;
#if DEBUG
using YBFramework.MyEditor;
#endif

namespace YBFramework.Component
{
    public sealed class Graph : IComponent
    {
        private Entity m_Owner;

        private readonly List<BaseNode> m_Nodes = new();

        public Entity GetOwner()
        {
            return m_Owner;
        }

        public void OnRemoveComponent()
        {
        }

        public void OnAddComponent(Entity entity)
        {
            m_Owner = entity;
        }

        public void ResetComponent()
        {
        }

        public void Revert(GraphAsset graphAsset)
        {
            //TODO:这一步提出到更早的初始化
            PortConnectHelper.RegisterWrapMethod();
            IReadOnlyList<NodeAsset> nodeAssets = graphAsset.GetNodeAssets();
            for (int i = 0; i < nodeAssets.Count; i++)
            {
                BaseNode node = nodeAssets[i].GetNode().Clone();
                if (node != null)
                {
                    m_Nodes.Add(node);
                }
            }
            for (int i = 0; i < m_Nodes.Count; i++)
            {
                BaseNode fromNode = m_Nodes[i];
                foreach (BasePort basePort in fromNode.GetPortEnumerable())
                {
                    if (basePort is IPortConnectionSource portConnectionSource)
                    {
                        portConnectionSource.Connect(m_Nodes);
                    }
                }
            }
        }

        public void Start()
        {
#if DEBUG
            GraphDebugHelper.AddRunningGraph(this);
#endif
            for (int i = 0; i < m_Nodes.Count; i++)
            {
                m_Nodes[i].OnStart();
            }
        }

        public void Stop()
        {
#if DEBUG
            GraphDebugHelper.RemoveRunningGraph(this);
#endif
            for (int i = 0; i < m_Nodes.Count; i++)
            {
                m_Nodes[i].OnStop();
            }
        }
    }
}