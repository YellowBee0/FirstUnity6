using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;

namespace YBFramework.MyEditor.Common
{
    public class ManagedReferenceListView<T> : VisualElement
    {
        private readonly List<Type> m_SubTypes = new();

        private readonly SerializedProperty m_ListProperty;

        private readonly ListView m_ListView;

        public ManagedReferenceListView(string title, List<T> list, SerializedProperty listProperty)
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
                    headerTitle = title,
                    reorderable = true,
                    showBorder = true,
                    showFoldoutHeader = true,
                    showBoundCollectionSize = true,
                    showAddRemoveFooter = true,
                    showAlternatingRowBackgrounds = AlternatingRowBackground.ContentOnly,
                    selectionType = SelectionType.Multiple,
                    virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight
                };
                m_ListView.onAdd += OnAddClick;
                m_ListView.onRemove += OnRemoveClick;
                Add(m_ListView);
            }
        }

        private VisualElement MakeItem()
        {
            return new ManagedReferencePropertyView(m_SubTypes);
        }

        private void BindElement(VisualElement element, int index)
        {
            if (element is ManagedReferencePropertyView managedReferencePropertyView)
            {
                managedReferencePropertyView.Bind(m_ListProperty.GetArrayElementAtIndex(index));
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
                Span<int> indexes = stackalloc int[m_ListProperty.arraySize];
                int count = 0;
                foreach (int index in m_ListView.selectedIndices)
                {
                    int insertIndex = count;
                    for (int i = 0; i < count; i++)
                    {
                        if (index > indexes[i])
                        {
                            insertIndex = i;
                            break;
                        }
                    }
                    for (int i = count - 1; i >= insertIndex; i--)
                    {
                        indexes[i + 1] = indexes[i];
                    }
                    indexes[insertIndex] = index;
                    count++;
                }
                for (int i = 0; i < count; i++)
                {
                    m_ListProperty.DeleteArrayElementAtIndex(indexes[i]);
                }
                m_ListView.ClearSelection();
            }
            m_ListProperty.serializedObject.ApplyModifiedProperties();
            listView.RefreshItems();
        }
    }
}