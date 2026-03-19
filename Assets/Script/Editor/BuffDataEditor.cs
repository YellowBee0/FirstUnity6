using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;
using YBFramework.Component;
using YBFramework.MyEditor.Common;

namespace YBFramework.MyEditor
{
    [CustomEditor(typeof(BuffData))]
    public class BuffDataEditor : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new();
            TextField buffNameField = new("BUFF名")
            {
                isDelayed = true,
                bindingPath = "m_BuffName"
            };
            EnumField buffTypeField = new("BUFF类型")
            {
                bindingPath = "m_BuffType"
            };
            root.Add(buffNameField);
            root.Add(buffTypeField);
            List<Type> typeOptions = new();
            TypeCache.TypeCollection types = TypeCache.GetTypesDerivedFrom<RepeatAddProcess>();
            foreach (Type type in types)
            {
                if (!type.IsAbstract)
                {
                    typeOptions.Add(type);
                }
            }
            Box box = new();
            ManagedReferencePropertyView view = new(typeOptions);
            view.Bind(serializedObject.FindProperty("m_RepeatAddProcess"));
            box.Add(view);
            root.Add(box);
            BuffData data = target as BuffData;
            if (data != null)
            {
                ManagedReferenceListView<BuffBehaviour> listView = new("Buff行为", data.m_Behaviours ??= new List<BuffBehaviour>(), serializedObject.FindProperty("m_Behaviours"));
                root.Add(listView);
            }
            return root;
        }
    }
}