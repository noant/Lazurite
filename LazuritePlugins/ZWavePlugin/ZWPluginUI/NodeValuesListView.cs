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
                else
                    GetItems().All(x => x.Selected = false);
            }
        }

        private ValueGenre? _genre = null;
        public ValueGenre? SelectedGenre
        {
            get
            {
                return _genre;
            }
            set
            {
                _genre = value;
                foreach (NodeValueView item in this.GetItems())
                    item.Visibility = _genre == item.NodeValue.Genre || _genre == null ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            }
        }

        public void RefreshWith(Node node, Func<NodeValue, bool> comparability)
        {
            var selected = SelectedNodeValue;
            this.Children.Clear();
            foreach (var nodeValue in node.Values.OrderBy(x=>x.Genre))
                this.Children.Add(new NodeValueView(nodeValue, comparability));
            SelectedNodeValue = selected;
            SelectedGenre = SelectedGenre; //crutch
        }
    }
}