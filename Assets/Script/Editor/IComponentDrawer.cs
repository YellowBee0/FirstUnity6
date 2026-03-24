using UnityEditor;
using UnityEngine.UIElements;
using YBFramework.Component;
using YBFramework.MyEditor.Common;

namespace YBFramework.MyEditor
{
    [CustomPropertyDrawer(typeof(IComponent))]
    public sealed class IComponentDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            ManagedReferencePropertyView propertyView = new(typeof(IComponent), null);
            propertyView.Bind(property);
            return propertyView;
        }
    }
}