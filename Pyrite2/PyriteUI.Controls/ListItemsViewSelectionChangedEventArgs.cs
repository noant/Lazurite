using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PyriteUI.Controls
{
    public class ListItemsViewSelectionChangedEventArgs : RoutedEventArgs
    {
        public ItemView[] SelectedItems { get; internal set; }
        public ListItemsView ListItemsView { get; internal set; }
    }
}
