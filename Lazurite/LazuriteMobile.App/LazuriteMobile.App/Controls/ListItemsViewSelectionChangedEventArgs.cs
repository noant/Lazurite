using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LazuriteMobile.App.Controls
{
    public class ListItemsViewSelectionChangedEventArgs : EventArgs
    {
        public ISelectable[] SelectedItems { get; internal set; }
        public ListItemsView ListItemsView { get; internal set; }
    }
}
