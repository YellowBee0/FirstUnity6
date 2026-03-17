using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using YBFramework.Component;

namespace YBFramework.MyEditor
{
    [CustomEditor(typeof(BUFFData))]
    public class BUFFDataEditor : UnityEditor.Editor
    {
        private static readonly List<int> s_RemovedItemIndex = new();

        private static List<BUFFBehaviourData> s_BehaviourData;

        private readonly List<SerializedProperty> m_ListData = new();

        private SerializedProperty m_ListProperty;

        private ListView m_ListView;

        public override VisualElement CreateInspectorGUI()
        {
            m_ListProperty = serializedObject.FindProperty("m_BUFFName");
            Debug.LogError($"{m_ListProperty.name} {m_ListProperty.displayName}");
            return (target as BUFFData).CreateBUFFDataView();
        }

        private VisualElement MakeListItem()
        {
            PropertyField propertyField = new();
            return propertyField;
        }

        private void BindListItem(VisualElement element, int index)
        {
            SerializedProperty property = m_ListProperty.GetArrayElementAtIndex(index);
            (element as PropertyField).BindProperty(property);
        }

        private void OnAddClick()
        {
            GenericMenu menu = new();
            foreach (BUFFBehaviourData behaviourData in s_BehaviourData)
            {
                menu.AddItem(new GUIContent(behaviourData.DisplayName), false, () =>
                {
                    m_ListProperty.arraySize++;
                    int index = m_ListProperty.arraySize - 1;
                    SerializedProperty newElement = m_ListProperty.GetArrayElementAtIndex(index);
                    newElement.managedReferenceValue = Activator.CreateInstance(behaviourData.BehaviourType);
                    serializedObject.ApplyModifiedProperties();
                    m_ListData.Add(newElement);
                    m_ListView.RefreshItems();
                });
            }
            menu.ShowAsContext();
        }

        private void OnRemoveClick()
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
                m_ListData.RemoveAt(index);
            }
            serializedObject.ApplyModifiedProperties();
            m_ListView.ClearSelection();
            m_ListView.RefreshItems();
        }

        private sealed class BUFFBehaviourData
        {
            public readonly Type BehaviourType;
            public readonly string DisplayName;

            public BUFFBehaviourData(string displayName, Type behaviourType)
            {
                DisplayName = displayName;
                BehaviourType = behaviourType;
            }
        }
    }
}