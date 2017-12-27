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
            this.SelectionMode = ListViewItemsSelectionMode.Single;
        }

        public Node SelectedNode
        {
            get
            {
                return (this.SelectedItem as NodeView)?.Node;
            }
            set
            {
                var obj = this.GetItems().FirstOrDefault(x => ((NodeView)x).Node.Equals(value));
                if (obj != null)
                    obj.Selected = true;
                else
                    this.GetItems().All(x => x.Selected = false);
            }
        }

        public void RefreshWith(Node[] nodes)
        {
            var selectedNode = SelectedNode;
            this.Children.Clear();
            foreach (var node in nodes)
                this.Children.Add(new NodeView(node));
            this.SelectedNode = selectedNode;
        }
    }
}
