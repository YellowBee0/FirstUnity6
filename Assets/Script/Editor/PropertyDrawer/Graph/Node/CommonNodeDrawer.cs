using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using YBFramework.Component;
using YBFramework.MyEditor.Common;

namespace YBFramework.MyEditor
{
    [Drawer(typeof(BaseNode))]
    public class CommonNodeDrawer
    {
        public virtual void DrawNodeView(NodeView nodeView, BaseNode node)
        {
            foreach (BasePort portDrawTarget in node.GetPortDrawTargetEnumerable())
            {
                if (DrawerManager.Allocate(portDrawTarget.GetType()) is CommonPortDrawer portDrawer)
                {
                    VisualElement element = portDrawer.DrawPortView(nodeView, portDrawTarget);
                    element.CommonBorder();
                    if (portDrawTarget.GetDirection() == Direction.Input)
                    {
                        nodeView.inputContainer.Add(element);
                    }
                    else
                    {
                        nodeView.outputContainer.Add(element);
                    }
                }
            }
        }
    }
}