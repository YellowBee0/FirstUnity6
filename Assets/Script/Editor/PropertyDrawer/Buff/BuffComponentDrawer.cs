using UnityEditor;
using UnityEngine.UIElements;
using YBFramework.Component;
using YBFramework.MyEditor.Common;

namespace YBFramework.MyEditor
{
    [CustomPropertyDrawer(typeof(IBuffComponent))]
    public class BuffComponentDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            ManagedReferencePropertyView propertyView = new(typeof(IBuffComponent), null);
            propertyView.Bind(property);
            return propertyView;
        }
    }
}