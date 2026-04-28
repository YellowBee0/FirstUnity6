using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using YBFramework.Component;

namespace YBFramework.MyEditor
{
    public partial class NewGraphWindow
    {
        private readonly Dictionary<GraphType, NewNodeSearchView> m_NodeSearchViews = new();

        private readonly Dictionary<string, Type> s_NodeTypes = new();

        private readonly NodeMenuBranch s_Root = new("Root", 0);

        private sealed class NodeMenuBranch : NodeMenuOption
        {
            private readonly List<NodeMenuOption> m_Options = new();

            public NodeMenuBranch(string name, int level) : base(name, level)
            {
            }

            public void AddOption(NodeMenuOption option)
            {
                m_Options.Add(option);
            }

            public IReadOnlyList<NodeMenuOption> GetOptions()
            {
                return m_Options;
            }
        }

        private sealed class NodeMenuLeaf : NodeMenuOption
        {
            public readonly GraphType GraphType;

            public NodeMenuLeaf(GraphType graphType, string name, int level) : base(name, level)
            {
                GraphType = graphType;
            }
        }

        private abstract class NodeMenuOption
        {
            public readonly int Level;

            public readonly string OptionText;

            protected NodeMenuOption(string name, int level)
            {
                OptionText = name;
                Level = level;
            }
        }

        private static bool RecursionSetSearchTreeEntry(NodeMenuBranch branch, GraphType graphType, in List<SearchTreeEntry> results)
        {
            bool hasAddToResult = false;
            IReadOnlyList<NodeMenuOption> options = branch.GetOptions();
            for (int i = 0; i < options.Count; i++)
            {
                if (options[i] is NodeMenuLeaf menuLeaf)
                {
                    if ((menuLeaf.GraphType & graphType) != 0)
                    {
                        results.Add(new SearchTreeEntry(new GUIContent(menuLeaf.OptionText))
                        {
                            level = menuLeaf.Level
                        });
                        hasAddToResult = true;
                    }
                }
                else
                {
                    results.Add(new SearchTreeGroupEntry(new GUIContent(options[i].OptionText), options[i].Level));
                    if (RecursionSetSearchTreeEntry(options[i] as NodeMenuBranch, graphType, in results))
                    {
                        hasAddToResult = true;
                    }
                    else
                    {
                        results.RemoveAt(results.Count - 1);
                    }
                }
            }
            return hasAddToResult;
        }

        private void InitNodeSearchMenu()
        {
            TypeCache.TypeCollection types = TypeCache.GetTypesDerivedFrom<BaseNode>("Assembly-CSharp");
            foreach (Type type in types)
            {
                NodeMenuAttribute nodeMenuAttribute = type.GetCustomAttribute<NodeMenuAttribute>();
                if (nodeMenuAttribute != null)
                {
                    ReadOnlySpan<char> menuSpan = nodeMenuAttribute.NodeName.AsSpan();
                    NodeMenuBranch curBranch = s_Root;
                    string optionText;
                    int startIndex = 0;
                    int sliceLenght = 0;
                    int level = 0;
                    IReadOnlyList<NodeMenuOption> options;
                    for (int i = 0; i < menuSpan.Length; i++)
                    {
                        if (menuSpan[i] == '/')
                        {
                            level++;
                            optionText = menuSpan.Slice(startIndex, sliceLenght).ToString();
                            NodeMenuOption optionTemp = null;
                            options = curBranch.GetOptions();
                            for (int j = 0; j < options.Count; j++)
                            {
                                if (options[j].OptionText == optionText && options[j] is NodeMenuBranch)
                                {
                                    optionTemp = options[j];
                                    break;
                                }
                            }
                            if (optionTemp == null)
                            {
                                optionTemp = new NodeMenuBranch(optionText, level);
                                curBranch.AddOption(optionTemp);
                            }
                            curBranch = (optionTemp as NodeMenuBranch)!;
                            startIndex = i + 1;
                            sliceLenght = 0;
                        }
                        else
                        {
                            sliceLenght++;
                        }
                    }
                    optionText = $"{menuSpan[startIndex..].ToString()}({nodeMenuAttribute.GraphType})";
                    options = curBranch.GetOptions();
                    for (int i = 0; i < options.Count; i++)
                    {
                        if (options[i].OptionText == optionText)
                        {
                            if (options[i] is NodeMenuLeaf)
                            {
                                Debug.LogWarning($"you may have the same node menu {optionText}");
                                return;
                            }
                        }
                    }
                    curBranch.AddOption(new NodeMenuLeaf(nodeMenuAttribute.GraphType, optionText, ++level));
                    s_NodeTypes.Add(optionText, type);
                }
            }
        }

        public List<SearchTreeEntry> GetSearchTreeEntries(GraphType graphType)
        {
            List<SearchTreeEntry> results = new()
            {
                new SearchTreeGroupEntry(new GUIContent(s_Root.OptionText), s_Root.Level)
            };
            RecursionSetSearchTreeEntry(s_Root, graphType, results);
            return results;
        }

        public Type MenuOptionToType(string menuOption)
        {
            return s_NodeTypes.GetValueOrDefault(menuOption);
        }

        private NewNodeSearchView GetSearchView(GraphType graphType)
        {
            if (!m_NodeSearchViews.TryGetValue(graphType, out NewNodeSearchView searchView))
            {
                searchView = CreateInstance<NewNodeSearchView>();
                searchView.Init(graphType);
                m_NodeSearchViews.Add(graphType, searchView);
            }
            return searchView;
        }
    }
}