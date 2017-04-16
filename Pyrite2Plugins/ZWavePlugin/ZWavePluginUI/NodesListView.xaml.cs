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
    /// Логика взаимодействия для NodesListView.xaml
    /// </summary>
    public partial class NodesListView : UserControl
    {
        public NodesListView()
        {
            InitializeComponent();
        }

        public void InitializeWith(ZWaveManager manager, Node selectedNode=null, Controller selectedController = null)
        {
            _manager = manager;
            captionView.StartAnimateProgress();
            listItems.Children.Clear();
            AppendAllNodes();
            SelectedNode = selectedNode;
            SelectedController = selectedController;
            captionView.StopAnimateProgress();

            SelectedNodeChanged?.Invoke(this, new RoutedEventArgs());
        }

        private void AppendAllNodes()
        {
            foreach (var node in _manager.GetNodes().Where(x=> SelectedController == null || x.Controller.Equals(SelectedController)))
            {
                var nodeView = new NodeView(node);
                listItems.Children.Add(nodeView);

                if (node.Equals(SelectedNode))
                    nodeView.Selected = true;

                nodeView.Click += (o, e) => {
                    SelectedNode = nodeView.Selected ? nodeView.Node : null;
                    SelectedNodeChanged?.Invoke(this, new RoutedEventArgs());
                };
            }
        }

        public Node SelectedNode { get; private set; }

        public Controller SelectedController { get; private set; }

        public event RoutedEventHandler SelectedNodeChanged;

        private ZWaveManager _manager;
    }
}
