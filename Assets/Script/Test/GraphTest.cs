using UnityEngine;
using YBFramework.Component;

namespace Script.Test
{
    public class GraphTest : Entity
    {
        public GraphAsset Asset;

        private Graph m_Graph;

        private void Awake()
        {
            m_Graph = new Graph();
            AddComponent(m_Graph);
            m_Graph.Revert(Asset);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                m_Graph.Invoke();
            }
        }
    }
}