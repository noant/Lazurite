using System.Windows;

namespace LazuriteUI.Windows.Controls
{
    public interface ISelectable
    {
        bool Selected { get; set; }
        bool Selectable { get; set; }
        event RoutedEventHandler SelectionChanged;
    }
}
