using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using YBFramework.MyEditor;
#endif

namespace YBFramework.Component
{
    [CreateAssetMenu(fileName = "New Graph Asset", menuName = "YBFramework/Graph Asset")]
    public sealed class GraphAsset : ScriptableObject
    {
        [SerializeField] private List<NodeAsset> m_NodeAssets = new();

        public IReadOnlyList<NodeAsset> GetNodeAssets()
        {
            return m_NodeAssets;
        }

        public IEnumerable<NodeAsset> NewGetNodeAssets()
        {
            return m_NodeAssets;
        }
#if UNITY_EDITOR
        [SerializeField] private GraphType m_GraphType = GraphType.Test1;

        [SerializeField] private int m_ID;

        public GraphType GetGraphType()
        {
            return m_GraphType;
        }

        public void AddNodeAsset(NodeAsset nodeAsset)
        {
            m_NodeAssets.Add(nodeAsset);
            AssetDatabase.AddObjectToAsset(nodeAsset, this);
            EditorUtility.SetDirty(this);
            AssetDatabase.Refresh();
        }

        public void RemoveNodeAsset(NodeAsset nodeAsset)
        {
            if (m_NodeAssets.Remove(nodeAsset))
            {
                AssetDatabase.RemoveObjectFromAsset(nodeAsset);
                EditorUtility.SetDirty(this);
                AssetDatabase.Refresh();
            }
        }

        public int AllocateID()
        {
            return m_ID++;
        }
#endif
    }
}