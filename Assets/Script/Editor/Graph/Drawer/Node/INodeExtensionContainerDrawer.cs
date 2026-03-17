using UnityEngine.UIElements;
using YBFramework.Component;

namespace YBFramework.MyEditor
{
    public interface INodeExtensionContainerDrawer
    {
        VisualElement DrawNodeView(NodeView nodeView, BaseNode node);
    }
}