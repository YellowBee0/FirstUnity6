using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace YBFramework.Component
{
    public sealed class NodeAsset : ScriptableObject
    {
        [SerializeReference] private BaseNode m_Node;

        public int GetNodeID()
        {
            return m_Node.ID;
        }
#if UNITY_EDITOR
        [SerializeField] private Vector2 m_Position;

        [SerializeField] private int m_ID;

        public BaseNode GetNode()
        {
            return m_Node;
        }

        public void SetNode(BaseNode node)
        {
            m_Node = node;
            SetSelfDirty();
        }

        public Vector2 GetPosition()
        {
            return m_Position;
        }

        public void SetPosition(Vector2 position)
        {
            m_Position = position;
            SetSelfDirty();
        }

        public int AllocateID()
        {
            return m_ID++;
        }

        public void SetSelfDirty()
        {
            EditorUtility.SetDirty(this);
        }
#endif
    }
}