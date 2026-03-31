using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;
using YBFramework.Component;

namespace YBFramework.MyEditor
{
    public partial class GraphWindow : EditorWindow
    {
        private static GraphWindow s_MainWindow;

        [MenuItem("Window/Graph")]
        public static void Open()
        {
            if (s_MainWindow == null)
            {
                s_MainWindow = GetWindow<GraphWindow>();
            }
            else
            {
                s_MainWindow.Focus();
            }
        }

        public static GraphWindow GetMainGraphWindow()
        {
            return s_MainWindow;
        }

        private void CreateGUI()
        {
            if (s_MainWindow != null)
            {
                s_MainWindow.Close();
            }
            s_MainWindow = this;
            string[] guids = AssetDatabase.FindAssets($"t:{nameof(GraphAsset)}");
            for (int i = 0; i < guids.Length; i++)
            {
                string graphPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                m_GraphPaths.Add(graphPath);
                m_GraphNames.Add(graphPath[(graphPath.LastIndexOf('/') + 1)..].Split('.')[0]);
            }
            m_FilteredResults = new List<string>(m_GraphNames);
            InitNodeSearchMenu();
            VisualElement graphAssetView = new()
            {
                style =
                {
                    maxWidth = 300,
                    minWidth = 100,
                    flexShrink = 0,
                    flexDirection = FlexDirection.Column,
                    borderRightWidth = 1,
                }
            };
            m_GraphContainer = new VisualElement
            {
                style =
                {
                    flexGrow = 1
                }
            };
            m_SelectedGraphPath = null;
            m_FilterStr = null;
            TextField searchField = new()
            {
                value = m_FilterStr
            };
            searchField.RegisterValueChangedCallback(OnSearchStrChanged);
            m_ListView = new ListView(m_FilteredResults, 20, MakeItem, BindItem);
            graphAssetView.Add(searchField);
            graphAssetView.Add(m_ListView);
            TwoPaneSplitView splitView = new(0, 200, TwoPaneSplitViewOrientation.Horizontal);
            splitView.Add(graphAssetView);
            splitView.Add(m_GraphContainer);
            rootVisualElement.Add(splitView);
        }
    }
}