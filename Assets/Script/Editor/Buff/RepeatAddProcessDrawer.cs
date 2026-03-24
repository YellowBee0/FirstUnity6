using UnityEditor;
using UnityEngine.UIElements;
using YBFramework.Component;
using YBFramework.MyEditor.Common;

namespace YBFramework.MyEditor
{
    [CustomPropertyDrawer(typeof(RepeatAddProcess))]
    public class RepeatAddProcessDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            ManagedReferencePropertyView propertyView = new(typeof(RepeatAddProcess), null);
            propertyView.Bind(property);
            return propertyView;
        }
    }
}