using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace YBFramework.Component
{
    public abstract class AnimationEventArgs
    {
#if UNITY_EDITOR
        public abstract VisualElement DrawGUI(SerializedProperty property);
#endif
    }

    [Serializable]
    public sealed class IntStringArg : AnimationEventArgs
    {
        public int IntArg;

        public string StringArg;

#if UNITY_EDITOR
        public override VisualElement DrawGUI(SerializedProperty property)
        {
            VisualElement container = new();
            IntegerField integerField = new("int参数");
            TextField textField = new("string参数");
            integerField.BindProperty(property.FindPropertyRelative(nameof(IntArg)));
            textField.BindProperty(property.FindPropertyRelative(nameof(StringArg)));
            container.Add(integerField);
            container.Add(textField);
            return container;
        }
#endif
    }

    [Serializable]
    public sealed class IntArg : AnimationEventArgs
    {
        public int Arg;

#if UNITY_EDITOR
        public override VisualElement DrawGUI(SerializedProperty property)
        {
            VisualElement container = new();
            IntegerField integerField = new("int参数");
            integerField.BindProperty(property.FindPropertyRelative(nameof(Arg)));
            container.Add(integerField);
            return container;
        }
#endif
    }
}