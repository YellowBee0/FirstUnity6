using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;

namespace YBFramework.MyEditor.Common
{
    public sealed class DynamicListViewDrawer<T>
    {
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
                    virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight
                };
                m_ListView.onAdd += OnAddClick;
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
                dynamicElement.Bind(m_ListProperty, index);
            }
        }

        private void OnAddClick(BaseListView listView)
        {
            m_ListProperty.serializedObject.Update();
            int index = m_ListProperty.arraySize;
            m_ListProperty.InsertArrayElementAtIndex(index);
            m_ListProperty.serializedObject.ApplyModifiedProperties();
        }
    }
}