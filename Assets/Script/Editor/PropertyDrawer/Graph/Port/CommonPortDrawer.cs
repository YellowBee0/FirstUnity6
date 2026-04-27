using System;
using UnityEngine.UIElements;
using YBFramework.Component;
using YBFramework.MyEditor.Common;

namespace YBFramework.MyEditor
{
    [Drawer(typeof(BasePort))]
    public class CommonPortDrawer
    {
        public virtual VisualElement DrawPortView(NodeView nodeView, BasePort target)
        {
            /*PortView portView = new(nodeView, target, portViewInfo.Name, portViewInfo.Direction, portViewInfo.Capacity, portViewInfo.Color, this);
            nodeView.AddPortView(portView);
            return portView;*/
            throw new NotImplementedException();
        }
    }
}