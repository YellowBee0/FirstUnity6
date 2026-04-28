using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using YBFramework.MyEditor;
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

        private SerializedObject m_SerializedObject;

        public void CreateNodeView(NewCustomGraphView graphView)
        {
            NewNodeView nodeView = new(this, graphView)
            {
                title = name
            };
            nodeView.SetPosition(new Rect(m_Position, Vector2.zero));
            m_Node.InitNodeViewInfo();
            m_SerializedObject = new SerializedObject(this);
            m_SerializedObject.Update();
            m_Node.FillNodeContentView(m_SerializedObject.FindProperty(nameof(m_Node)), nodeView);
            graphView.AddElement(nodeView);
        }

        public BaseNode GetNode()
        {
            return m_Node;
        }

        public void SetNode(BaseNode node)
        {
            m_Node = node;
        }

        public Vector2 GetPosition()
        {
            return m_Position;
        }

        public void SetPosition(Vector2 position)
        {
            m_Position = position;
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