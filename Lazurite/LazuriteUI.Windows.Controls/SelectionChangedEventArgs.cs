using System.Windows;

namespace LazuriteUI.Windows.Controls
{
    class SelectionChangedEventArgs: RoutedEventArgs
    {
        public ISelectable Item { get; internal set; }
        public bool Selected { get; internal set; }
    }
}
