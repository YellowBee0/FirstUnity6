using System;
using System.Collections.Generic;
using UnityEngine;
using YBFramework.Common;
#if DEBUG
using YBFramework.MyEditor;
#endif

namespace YBFramework.Component
{
#if UNITY_EDITOR
    [DisplayName("蓝图")]
#endif
    [Serializable]
    public sealed class Graph : IComponent
    {
        private Entity m_Owner;

        [SerializeField] private GraphAsset m_GraphAsset;

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

        public IComponent Clone()
        {
            Graph graph = new()
            {
                m_GraphAsset = m_GraphAsset
            };
            return graph;
        }

        public void Revert()
        {
            //TODO:这一步提出到更早的初始化
            PortConnectHelper.RegisterWrapMethod();
            IReadOnlyList<NodeAsset> nodeAssets = m_GraphAsset.GetNodeAssets();
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