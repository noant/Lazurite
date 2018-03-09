using LazuriteUI.Windows.Controls;
using OpenZWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZWPluginUI
{
    public class NodesListView: ListItemsView
    {
        public NodesListView()
        {
            SelectionMode = ListViewItemsSelectionMode.Single;
        }

        public Node SelectedNode
        {
            get
            {
                return (SelectedItem as NodeView)?.Node;
            }
            set
            {
                var obj = GetItems().FirstOrDefault(x => ((NodeView)x).Node.Equals(value));
                if (obj != null)
                    obj.Selected = true;
                else
                    GetItems().All(x => x.Selected = false);
            }
        }

        public void RefreshWith(Node[] nodes)
        {
            var selectedNode = SelectedNode;
            Children.Clear();
            foreach (var node in nodes)
                Children.Add(new NodeView(node));
            SelectedNode = selectedNode;
        }
    }
}
