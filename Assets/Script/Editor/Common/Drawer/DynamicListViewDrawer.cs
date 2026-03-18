using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;

namespace YBFramework.MyEditor.Common
{
    public sealed class DynamicListViewDrawer<T>
    {
        private static readonly List<int> s_RemovedItemIndex = new();

        private readonly List<Type> m_SubTypes = new();

        private SerializedProperty m_ListProperty;

        private List<T> m_List;

        private ListView m_ListView;

        public ListView Draw(List<T> list, SerializedProperty listProperty)
        {
            if (listProperty.isArray)
            {
                m_ListProperty = listProperty;
                TypeCache.TypeCollection types = TypeCache.GetTypesDerivedFrom<T>();
                foreach (Type type in types)
                {
                    if (!type.IsAbstract)
                    {
                        m_SubTypes.Add(type);
                    }
                }
                m_ListView = new ListView(list, -1, MakeItem, BindElement)
                {
                    showBorder = true,
                    showAddRemoveFooter = true,
                    selectionType = SelectionType.Multiple,
                    virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight
                };
                m_ListView.onAdd += OnAddClick;
                m_ListView.onRemove += OnRemoveClick;
            }
            return m_ListView;
        }

        private VisualElement MakeItem()
        {
            return new DynamicElementView(m_SubTypes);
        }

        private void BindElement(VisualElement element, int index)
        {
            if (element is DynamicElementView dynamicElement)
            {
                dynamicElement.Bind(m_ListProperty.GetArrayElementAtIndex(index));
            }
        }

        private void OnAddClick(BaseListView listView)
        {
            m_ListProperty.serializedObject.Update();
            m_ListProperty.arraySize++;
            m_ListProperty.serializedObject.ApplyModifiedProperties();
        }

        private void OnRemoveClick(BaseListView listView)
        {
            m_ListProperty.serializedObject.Update();
            int selectedIndex = m_ListView.selectedIndex;
            if (selectedIndex == -1)
            {
                m_ListProperty.arraySize--;
            }
            else
            {
                s_RemovedItemIndex.Clear();
                foreach (int index in m_ListView.selectedIndices)
                {
                    int insertIndex = 0;
                    for (int i = 0; i < s_RemovedItemIndex.Count; i++)
                    {
                        if (index > s_RemovedItemIndex[i])
                        {
                            insertIndex = i;
                            break;
                        }
                    }
                    s_RemovedItemIndex.Insert(insertIndex, index);
                }
                for (int i = 0; i < s_RemovedItemIndex.Count; i++)
                {
                    int index = s_RemovedItemIndex[i];
                    m_ListProperty.DeleteArrayElementAtIndex(index);
                    if (selectedIndex != index)
                    {
                        m_List.RemoveAt(index);
                    }
                }
            }
            m_ListProperty.serializedObject.ApplyModifiedProperties();
        }
    }
}