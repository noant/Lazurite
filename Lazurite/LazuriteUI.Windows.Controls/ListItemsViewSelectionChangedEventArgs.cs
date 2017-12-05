using System.Windows;

namespace LazuriteUI.Windows.Controls
{
    public class ListItemsViewSelectionChangedEventArgs : RoutedEventArgs
    {
        public ISelectable[] SelectedItems { get; internal set; }
        public ListItemsView ListItemsView { get; internal set; }
    }
}
