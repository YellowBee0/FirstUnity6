using UnityEngine.UIElements;
using YBFramework.Component;
using YBFramework.MyEditor.Common;

namespace YBFramework.MyEditor
{
    [Drawer(typeof(PortDrawTarget))]
    public class CommonPortDrawer
    {
        public virtual VisualElement DrawPortView(NodeView nodeView, PortDrawTarget target)
        {
            BasePort basePort = (BasePort)target;
            PortViewInfo portViewInfo = target.PortViewInfo;
            PortView portView = new(nodeView, basePort, portViewInfo.Name, portViewInfo.Direction, portViewInfo.Capacity, portViewInfo.Color);
            nodeView.AddPortView(portView);
            return portView;
        }
    }
}