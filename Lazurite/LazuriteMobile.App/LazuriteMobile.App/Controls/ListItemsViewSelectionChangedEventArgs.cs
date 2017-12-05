using System;

namespace LazuriteMobile.App.Controls
{
    public class ListItemsViewSelectionChangedEventArgs : EventArgs
    {
        public ISelectable[] SelectedItems { get; internal set; }
        public ListItemsView ListItemsView { get; internal set; }
    }
}
