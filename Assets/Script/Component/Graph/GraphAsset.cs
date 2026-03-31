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
#if UNITY_EDITOR
        [HideInInspector] [SerializeField] private GraphType m_GraphType = GraphType.Test1;

        [HideInInspector] [SerializeField] private int m_ID;

        public GraphType GetGraphType()
        {
            return m_GraphType;
        }

        public void AddNodeAsset(NodeAsset nodeAsset)
        {
            m_NodeAssets.Add(nodeAsset);
            EditorUtility.SetDirty(this);
            AssetDatabase.AddObjectToAsset(nodeAsset, this);
            AssetDatabase.Refresh();
        }

        public void RemoveNodeAsset(NodeAsset nodeAsset)
        {
            if (m_NodeAssets.Remove(nodeAsset))
            {
                EditorUtility.SetDirty(this);
                AssetDatabase.RemoveObjectFromAsset(nodeAsset);
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