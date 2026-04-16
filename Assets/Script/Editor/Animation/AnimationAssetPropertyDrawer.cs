using UnityEditor;
using UnityEngine.UIElements;
using YBFramework.Component;

namespace YBFramework.MyEditor
{
    [CustomPropertyDrawer(typeof(AnimationEventArgs))]
    public sealed class AnimationAssetPropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            AnimationEventArgs args = property.managedReferenceValue as AnimationEventArgs;
            return args?.DrawGUI(property);
        }
    }
}