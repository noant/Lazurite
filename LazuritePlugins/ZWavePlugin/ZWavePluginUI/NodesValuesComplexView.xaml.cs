using OpenZWrapper;
using System;
using System.Windows;
using System.Windows.Controls;

namespace ZWavePluginUI
{
    /// <summary>
    /// Логика взаимодействия для NodesValuesComplexView.xaml
    /// </summary>
    public partial class NodesValuesComplexView : UserControl, IRefreshable, ICanBlockUI
    {
        public NodesValuesComplexView()
        {
            InitializeComponent();
            nodesListView.SelectedNodeChanged += (o, e) =>
                valuesListView.InitializeWith(nodesListView.SelectedNode, null, _nodeValueFilter);
            valuesListView.SelectionChanged += (o, e) =>
            {
                SelectedNodeValue = valuesListView.SelectedNodeValue;
                SelectionChanged?.Invoke(this, new RoutedEventArgs());
                IsDataAllowed?.Invoke(SelectedNodeValue != null);
            };
        }

        public bool AllowChangeRange {
            get
            {
                return valuesListView.AllowChangeRange;
            }
            set
            {
                valuesListView.AllowChangeRange = value;
            }
        }
        
        public void InitializeWith(ZWaveManager manager, Node selectedNode=null, NodeValue selectedNodeValue=null, Func<NodeValue, bool> nodeValueFilter=null)
        {
            _nodeValueFilter = nodeValueFilter;
            _manager = manager;
            SelectedNode = selectedNode;
            SelectedNodeValue = selectedNodeValue;
            Refresh();
        }
        
        public void Refresh()
        {
            nodesListView.InitializeWith(_manager, SelectedNode);
            valuesListView.InitializeWith(SelectedNode, SelectedNodeValue, _nodeValueFilter);
            IsDataAllowed?.Invoke(false);
        }

        public NodeValue SelectedNodeValue { get; private set; }
        public Node SelectedNode { get; private set; }
        private ZWaveManager _manager;

        public Action<bool> IsDataAllowed
        {
            get;
            set;
        }

        public Action NeedClose
        {
            get;
            set;
        }

        public Action<bool> BlockUI
        {
            get;
            set;
        }

        public event RoutedEventHandler SelectionChanged;
        private Func<NodeValue, bool> _nodeValueFilter;
    }
}
