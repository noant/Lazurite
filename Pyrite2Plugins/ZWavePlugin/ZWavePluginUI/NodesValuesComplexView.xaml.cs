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
    public partial class NodesValuesComplexView : UserControl
    {
        public NodesValuesComplexView()
        {
            InitializeComponent();
            okItem.IsEnabled = false;
            nodesListView.SelectedNodeChanged += (o, e) => valuesListView.InitializeWith(nodesListView.SelectedNode);
            valuesListView.SelectionChanged += (o, e) =>
            {
                SelectedNodeValue = valuesListView.SelectedNodeValue;
                SelectionChanged?.Invoke(this, new RoutedEventArgs());

                okItem.IsEnabled = SelectedNodeValue != null;
            };
        }

        public void InitializeWith(ZWaveManager manager, Node selectedNode=null, NodeValue selectedNodeValue=null)
        {
            nodesListView.InitializeWith(manager, selectedNode);
            valuesListView.InitializeWith(selectedNode, selectedNodeValue);
        }

        private void OkClick(object sender, RoutedEventArgs e)
        {
            OkClicked?.Invoke(this, new RoutedEventArgs());
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            CancelClicked?.Invoke(this, new RoutedEventArgs());
        }

        public NodeValue SelectedNodeValue { get; private set; }
        public event RoutedEventHandler SelectionChanged;
        public event RoutedEventHandler OkClicked;
        public event RoutedEventHandler CancelClicked;
    }
}
