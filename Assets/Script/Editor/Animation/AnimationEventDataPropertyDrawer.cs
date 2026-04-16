using UnityEditor;
using UnityEngine.UIElements;
using YBFramework.Component;
using YBFramework.MyEditor.Common;

namespace YBFramework.MyEditor
{
    [CustomPropertyDrawer(typeof(AnimationEventData))]
    public sealed class AnimationEventDataPropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            SerializedProperty eventArgsProperty = property.FindPropertyRelative("EventArgs");
            ManagedReferencePropertyView propertyView = new(typeof(AnimationEventArgs), null);
            propertyView.Bind(eventArgsProperty);
            return propertyView;
        }
    }
}