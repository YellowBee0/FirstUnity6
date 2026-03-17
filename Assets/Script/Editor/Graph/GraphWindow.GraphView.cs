using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using YBFramework.Component;

namespace YBFramework.MyEditor
{
    public partial class GraphWindow
    {
        private readonly Dictionary<string, CustomGraphView> m_DrawnGraphs = new();

        private readonly List<string> m_GraphNames = new();

        private readonly List<string> m_GraphPaths = new();

        private List<string> m_FilteredResults;

        private VisualElement m_GraphContainer;

        private CustomGraphView m_SelectedGraphView;

        private ListView m_ListView;

        private string m_SelectedGraphPath;

        private string m_FilterStr;

        private static VisualElement MakeItem()
        {
            return new Label
            {
                //设置内容超出父元素后的显示方式
                style =
                {
                    overflow = Overflow.Hidden,
                    whiteSpace = WhiteSpace.NoWrap,
                    textOverflow = TextOverflow.Ellipsis
                }
            };
        }

        private void OnSearchStrChanged(ChangeEvent<string> evt)
        {
            if (m_FilterStr != evt.newValue)
            {
                m_FilterStr = evt.newValue;
                m_FilteredResults.Clear();
                if (string.IsNullOrEmpty(m_FilterStr))
                {
                    m_FilteredResults.AddRange(m_GraphNames);
                }
                else
                {
                    for (int i = 0; i < m_GraphNames.Count; i++)
                    {
                        if (m_GraphNames[i].Contains(m_FilterStr, StringComparison.OrdinalIgnoreCase))
                        {
                            m_FilteredResults.Add(m_GraphNames[i]);
                        }
                    }
                }
                m_ListView.RefreshItems();
            }
        }

        private void BindItem(VisualElement item, int index)
        {
            Label label = (item as Label)!;
            string graphName = m_FilteredResults[index];
            label.text = graphName;
            label.RegisterCallback<MouseDownEvent>(_ => { ChangeMainGraphView(m_GraphPaths[m_GraphNames.IndexOf(graphName)]); });
        }

        private void ChangeMainGraphView(string graphAssetPath)
        {
            if (m_SelectedGraphPath != graphAssetPath)
            {
                if (!m_DrawnGraphs.TryGetValue(graphAssetPath, out CustomGraphView graphView))
                {
                    GraphAsset graphAsset = AssetDatabase.LoadAssetAtPath<GraphAsset>(graphAssetPath);
                    if (graphAsset != null)
                    {
                        graphView = new CustomGraphView(graphAsset, s_MainWindow.GetSearchView(graphAsset.GetGraphType()));
                        m_DrawnGraphs.Add(graphAssetPath, graphView);
                    }
                    else
                    {
                        return;
                    }
                }
                m_SelectedGraphView?.RemoveFromHierarchy();
                m_SelectedGraphPath = graphAssetPath;
                m_SelectedGraphView = graphView;
                m_GraphContainer.Add(m_SelectedGraphView);
            }
        }

        public Vector2 GetPosition(SearchWindowContext context)
        {
            Vector2 worldPos = rootVisualElement.ChangeCoordinatesTo(rootVisualElement.parent, context.screenMousePosition - position.position);
            return m_SelectedGraphView.contentViewContainer.WorldToLocal(worldPos);
        }

        public void CreateNodeInMainGraphView(Type nodeType, string nodeName, Vector2 nodePosition)
        {
            if (nodeType == null)
            {
                return;
            }
            if (Activator.CreateInstance(nodeType) is BaseNode baseNode)
            {
                NodeAsset nodeAsset = CreateInstance<NodeAsset>();
                nodeAsset.name = nodeName;
                nodeAsset.SetNode(baseNode);
                nodeAsset.SetPosition(nodePosition);
                baseNode.SetID(m_SelectedGraphView.GraphAsset.AllocateID());
                foreach (BasePort basePort in baseNode.GetPortEnumerable())
                {
                    basePort.SetID(nodeAsset.AllocateID());
                }
                m_SelectedGraphView.AddNodeView(new NodeView(m_SelectedGraphView, nodeAsset));
            }
        }
    }
}