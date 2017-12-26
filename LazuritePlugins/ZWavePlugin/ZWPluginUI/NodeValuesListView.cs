using LazuriteUI.Windows.Controls;
using OpenZWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZWPluginUI
{
    public class NodeValuesListView: ListItemsView
    {
        public NodeValuesListView()
        {
            this.SelectionMode = ListViewItemsSelectionMode.Single;
        }

        public NodeValue SelectedNodeValue
        {
            get
            {
                return (this.SelectedItem as NodeValueView)?.NodeValue;
            }
            set
            {
                var nodeValueView = GetItems().Cast<NodeValueView>().FirstOrDefault(x => x.NodeValue.Equals(value));
                if (nodeValueView != null)
                    nodeValueView.Selected = true;
            }
        }

        public void RefreshWith(Node node)
        {
            var selected = SelectedNodeValue;
            this.Children.Clear();
            foreach (var nodeValue in node.Values)
                this.Children.Add(new NodeValueView(nodeValue));
            SelectedNodeValue = selected;
        }
    }
}
