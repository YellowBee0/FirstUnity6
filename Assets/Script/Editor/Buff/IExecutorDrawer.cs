using UnityEditor;
using UnityEngine.UIElements;
using YBFramework.Component;
using YBFramework.MyEditor.Common;

namespace YBFramework.MyEditor
{
    [CustomPropertyDrawer(typeof(IExecutor))]
    public class IExecutorDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            ManagedReferencePropertyView propertyView = new(typeof(IExecutor));
            propertyView.Bind(property);
            return propertyView;
        }
    }
}