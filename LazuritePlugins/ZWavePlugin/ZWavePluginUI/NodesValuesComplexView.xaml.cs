using OpenZWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ZWavePluginUI
{
    /// <summary>
    /// Логика взаимодействия для NodesValuesComplexView.xaml
    /// </summary>
    public partial class NodesValuesComplexView : UserControl, IRefreshable
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

        public event RoutedEventHandler SelectionChanged;
        private Func<NodeValue, bool> _nodeValueFilter;
    }
}
