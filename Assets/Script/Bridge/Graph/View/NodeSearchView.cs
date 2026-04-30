using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace YBFramework.MyEditor
{
    public sealed class NodeSearchView : ScriptableObject, ISearchWindowProvider
    {
        private List<SearchTreeEntry> m_SearchTreeEntries;

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            return m_SearchTreeEntries;
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            GraphWindow mainWindow = GraphWindow.GetMainGraphWindow();
            Type type = mainWindow.MenuOptionToType(SearchTreeEntry.name);
            mainWindow.CreateNodeInMainGraphView(type, SearchTreeEntry.name, mainWindow.GetPosition(context));
            return true;
        }

        public void Init(GraphType graphType)
        {
            m_SearchTreeEntries = GraphWindow.GetMainGraphWindow().GetSearchTreeEntries(graphType);
        }
    }
}