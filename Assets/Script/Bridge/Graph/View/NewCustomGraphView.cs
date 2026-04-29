#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using YBFramework.Component;

namespace YBFramework.MyEditor
{
    public sealed class NewCustomGraphView : GraphView
    {
        public readonly GraphAsset m_BindGraphAsset;

        private readonly List<Port> m_InputPortViews = new();

        private readonly List<Port> m_OutputPortViews = new();

        public NewCustomGraphView(GraphAsset bindGraphAsset, NewNodeSearchView nodeSearchView)
        {
            m_BindGraphAsset = bindGraphAsset;
            name = bindGraphAsset.name;
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            
            GridBackground grid = new();
            grid.StretchToParentSize();
            Insert(0, grid);

            this.StretchToParentSize();
            nodeCreationRequest = context => { SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), nodeSearchView); };
            //graphViewChanged += OnGraphViewChanged;
            
            foreach (NodeAsset nodeAsset in bindGraphAsset.NewGetNodeAssets())
            {
                nodeAsset.CreateNodeView(this);
            }
        }
        
        public void AddPortView(NewPortView portView)
        {
            if (portView.direction == Direction.Input)
            {
                m_InputPortViews.Add(portView);
            }
            else
            {
                m_OutputPortViews.Add(portView);
            }
        }

        public void RemovePortView(NewPortView portView)
        {
            if (portView.direction == Direction.Input)
            {
                m_InputPortViews.Remove(portView);
            }
            else
            {
                m_OutputPortViews.Remove(portView);
            }
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            return startPort.direction == Direction.Input ? m_OutputPortViews : m_InputPortViews;
        }
    }
}
#endif