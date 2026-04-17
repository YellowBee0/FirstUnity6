using UnityEditor;
using UnityEngine.UIElements;
using YBFramework.Component;

namespace YBFramework.MyEditor
{
    [CustomPropertyDrawer(typeof(AnimationEventArgs))]
    public sealed class AnimationEventArgsPropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            AnimationEventArgs eventArgs = property.managedReferenceValue as AnimationEventArgs;
            return eventArgs?.DrawGUI(property);
        }
    }
}