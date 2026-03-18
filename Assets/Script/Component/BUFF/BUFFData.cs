using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.UIElements;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.UIElements;
#endif

namespace YBFramework.Component
{
#if UNITY_EDITOR
    [CreateAssetMenu(fileName = "BUFFData", menuName = "BUFFData")]
#endif
    public sealed class BUFFData : ScriptableObject
    {
        [SerializeReference] public List<BUFFBehaviour> m_Behaviours = new();

        [SerializeField] private string m_BUFFName;

        [SerializeField] private BUFFType m_BUFFType;

        [SerializeField] private StackOption m_StackOption;

        [SerializeField] private int m_MaxLayer;

        [SerializeField] private int m_MinLayer;

        [SerializeField] private int m_InitialLayer;

        [SerializeField] private bool m_IsRemoveOnMaxLayer;

        [SerializeField] private bool m_IsRemoveOnMinLayer;

        [SerializeField] private bool m_IsRecordLayer;

        public IReadOnlyList<BUFFBehaviour> GetBehaviours()
        {
            return m_Behaviours;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string GetBUFFName()
        {
            return m_BUFFName;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BUFFType GetBUFFType()
        {
            return m_BUFFType;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public StackOption GetStackOption()
        {
            return m_StackOption;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMaxLayer()
        {
            return m_MaxLayer;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetMinLayer()
        {
            return m_MinLayer;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetInitialLayer()
        {
            return m_InitialLayer;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool GetIsRemoveOnMaxLayer()
        {
            return m_IsRemoveOnMaxLayer;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool GetIsRemoveOnMinLayer()
        {
            return m_IsRemoveOnMinLayer;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool GetIsRecordLayer()
        {
            return m_IsRecordLayer;
        }
#if UNITY_EDITOR
        private VisualElement m_LayerContainer;

        public VisualElement CreateBUFFDataView()
        {
            VisualElement root = new();
            TextField buffNameField = new TextField("BUFF名")
            {
                isDelayed = true,
                bindingPath = nameof(m_BUFFName)
            };
            IntegerField maxLayerField = new IntegerField("最大层数")
            {
                isDelayed = true,
                bindingPath = nameof(m_MaxLayer)
            };
            maxLayerField.RegisterValueChangedCallback(OnMaxLayerChanged);
            IntegerField minLayerField = new IntegerField("最小层数")
            {
                isDelayed = true,
                bindingPath = nameof(m_MinLayer)
            };
            minLayerField.RegisterValueChangedCallback(OnMinLayerChanged);
            IntegerField initialLayerField = new IntegerField("初始层数")
            {
                isDelayed = true,
                bindingPath = nameof(m_InitialLayer)
            };
            initialLayerField.RegisterValueChangedCallback(OnInitialLayerChanged);
            root.Add(maxLayerField);
            root.Add(minLayerField);
            root.Add(initialLayerField);
            root.Add(buffNameField);
            root.Bind(new SerializedObject(this));
            /*buffNameField.RegisterValueChangedCallback(OnBUFFNameChanged);
            EnumField buffTypeField = new EnumField("BUFF类型", m_BUFFType);
            buffTypeField.RegisterValueChangedCallback(OnBUFFTypeChanged);
            EnumField buffStackOptionField = new EnumField("BUFF堆叠类型", m_StackOption);
            buffStackOptionField.RegisterValueChangedCallback(OnBUFFStackOptionChanged);
            m_LayerContainer = new VisualElement();
            m_LayerContainer.SetEnabled(m_StackOption != StackOption.NotAllow);
            Toggle isRemoveOnMaxLayerField = new Toggle("达到最大层时移除")
            {
                value = m_IsRemoveOnMaxLayer
            };
            isRemoveOnMaxLayerField.RegisterValueChangedCallback(OnIsRemoveOnMaxLayerChanged);
            Toggle isRemoveOnMinLayerField = new Toggle("达到最小层时移除")
            {
                value = m_IsRemoveOnMinLayer
            };
            isRemoveOnMinLayerField.RegisterValueChangedCallback(OnIsRemoveOnMinLayerChanged);
            Toggle isRecordLayerField = new Toggle("记录层数改变")
            {
                value = m_IsRecordLayer
            };
            isRecordLayerField.RegisterValueChangedCallback(OnIsRecordLayerChanged);

            m_LayerContainer.Add(isRemoveOnMaxLayerField);
            m_LayerContainer.Add(isRemoveOnMinLayerField);
            m_LayerContainer.Add(isRecordLayerField);
            root.Add(buffNameField);
            root.Add(buffTypeField);
            root.Add(buffStackOptionField);*/
            root.Add(m_LayerContainer);
            return root;
        }

        private void OnBUFFNameChanged(ChangeEvent<string> changeEvent)
        {
            Undo.RecordObject(this, "Change BUFFName");
            m_BUFFName = changeEvent.newValue;
            EditorUtility.SetDirty(this);
        }

        private void OnBUFFTypeChanged(ChangeEvent<Enum> changeEvent)
        {
            Undo.RecordObject(this, "Change BUFFType");
            m_BUFFType = (BUFFType)changeEvent.newValue;
            EditorUtility.SetDirty(this);
        }

        private void OnBUFFStackOptionChanged(ChangeEvent<Enum> changeEvent)
        {
            Undo.RecordObject(this, "Change BUFFStackOption");
            m_StackOption = (StackOption)changeEvent.newValue;
            m_LayerContainer.SetEnabled(m_StackOption != StackOption.NotAllow);
            EditorUtility.SetDirty(this);
        }

        private void OnMaxLayerChanged(ChangeEvent<int> changeEvent)
        {
            m_MaxLayer = changeEvent.newValue < m_MinLayer ? m_MinLayer : changeEvent.newValue;
        }

        private void OnMinLayerChanged(ChangeEvent<int> changeEvent)
        {
            m_MinLayer = changeEvent.newValue > m_MaxLayer ? m_MaxLayer : changeEvent.newValue;
        }

        private void OnInitialLayerChanged(ChangeEvent<int> changeEvent)
        {
            int newValue = changeEvent.newValue;
            if (newValue > m_MaxLayer)
            {
                newValue = m_MaxLayer;
            }
            else if (newValue < m_MinLayer)
            {
                newValue = m_MinLayer;
            }
            m_InitialLayer = newValue;
        }

        private void OnIsRemoveOnMaxLayerChanged(ChangeEvent<bool> changeEvent)
        {
            Undo.RecordObject(this, "Change IsRemoveOnMaxLayer");
            m_IsRemoveOnMaxLayer = changeEvent.newValue;
            EditorUtility.SetDirty(this);
        }

        private void OnIsRemoveOnMinLayerChanged(ChangeEvent<bool> changeEvent)
        {
            Undo.RecordObject(this, "Change IsRemoveOnMinLayer");
            m_IsRemoveOnMinLayer = changeEvent.newValue;
            EditorUtility.SetDirty(this);
        }

        private void OnIsRecordLayerChanged(ChangeEvent<bool> changeEvent)
        {
            Undo.RecordObject(this, "Change IsRecordLayer");
            m_IsRecordLayer = changeEvent.newValue;
            EditorUtility.SetDirty(this);
        }
#endif
    }
}