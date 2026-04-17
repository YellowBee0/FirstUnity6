using UnityEditor;
using UnityEditor.UIElements;
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
            VisualElement root = new();
            TextField sourceNameField = new("来源");
            sourceNameField.BindProperty(property.FindPropertyRelative("SourceName"));
            root.Add(sourceNameField);
            FloatField triggerTimeField = new("触发时间");
            triggerTimeField.BindProperty(property.FindPropertyRelative("TriggerTime"));
            root.Add(triggerTimeField);
            ManagedReferencePropertyView eventArgsView = new(typeof(AnimationEventArgs), null);
            eventArgsView.Bind(property.FindPropertyRelative("EventArgs"));
            root.Add(eventArgsView);
            return root;
        }
    }
}