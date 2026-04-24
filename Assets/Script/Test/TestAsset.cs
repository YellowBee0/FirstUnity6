using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace YBFramework.Test
{
    [CreateAssetMenu(fileName = "New Test Asset", menuName = "YBFramework/Test Asset")]
    public sealed class TestAsset : ScriptableObject
    {
        public GenericClass<int> Value1;

        public GenericClass<float> Value2;

        public GenericClass<string> Value3;
        
        public Vector2Class Value4;
    }

    [Serializable]
    public class GenericClass<T>
    {
        public T Value;
    }

    [Serializable]
    public sealed class Vector2Class : GenericClass<Vector2>
    {
        public string Label;
    }

    [CustomPropertyDrawer(typeof(GenericClass<>))]
    public sealed class GenericPropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            return new PropertyField(property.FindPropertyRelative("Value"), "值:");
        }
    }

    [CustomPropertyDrawer(typeof(Vector2Class))]
    public sealed class Vector2ClassPropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            return new Label("this is vector2 value");
        }
    }
    //测试：PropertyDrawer能不能够绘制泛型类
    //结果：可以绘制，不过绘制泛型对象就必须使用PropertyField来绘制（因为确定泛型类型需要大量判断，不如直接使用Unity自带的东西）
}