using System.Collections.Generic;

namespace YBFramework.Component
{
    public sealed class Graph : IComponent
    {
        private readonly List<BaseNode> m_Nodes = new();

        private Entity m_Owner;

        public Entity GetOwner()
        {
            return m_Owner;
        }

        public void OnRemoveComponent()
        {
        }

        public void ResetComponent()
        {
        }

        public void OnAddComponent(Entity entity)
        {
            m_Owner = entity;
        }

        public void Invoke()
        {
            foreach (var node in m_Nodes)
            {
                if (node is DebugNode debugNode)
                {
                    debugNode.Log();
                    break;
                }
            }
        }

        public void Revert(GraphAsset graphAsset)
        {
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
    }
}