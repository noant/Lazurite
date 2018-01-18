using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace LazuriteUI.Windows.Controls
{
    /// <summary>
    /// Логика взаимодействия для ListItemsView.xaml
    /// </summary>
    public partial class ListItemsView : StackPanel
    {
        public static readonly DependencyProperty SelectionModeProperty;
        public static readonly DependencyProperty SelectedItemProperty;
        private bool _lockSelectionChangedEvent = false;
        
        static ListItemsView()
        {
            SelectionModeProperty = DependencyProperty.Register(nameof(SelectionMode), typeof(ListViewItemsSelectionMode), typeof(ListItemsView), new FrameworkPropertyMetadata()
            {
                PropertyChangedCallback = (o, e) =>
                {
                    var listItemsView = ((ListItemsView)o);
                    var mode = (ListViewItemsSelectionMode)e.NewValue;
                    foreach (var children in listItemsView.Children)
                    {
                        var item = children as ISelectable;
                        if (item != null)
                        {
                            item.Selected = false;
                            item.Selectable = mode != ListViewItemsSelectionMode.None;
                        }
                    }
                }
            });
            SelectedItemProperty = DependencyProperty.Register(nameof(SelectedItemProperty), typeof(ISelectable), typeof(ListItemsView), new FrameworkPropertyMetadata()
            {
                PropertyChangedCallback = (o, e) =>
                {
                    var item = e.NewValue as ISelectable;
                    if (item != null)
                        item.Selected = true;
                }
            });
        }

        public ListItemsView()
        {
            this.ClipToBounds = true;
            this.IsHitTestVisible = true;
        }

        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            if (visualAdded == null && visualRemoved != null)
                base.OnVisualChildrenChanged(null, visualRemoved);
            else
            {
                var item = visualAdded as ISelectable;
                if (item != null)
                {
                    item.Selectable = SelectionMode != ListViewItemsSelectionMode.None;
                    item.SelectionChanged += (o, e) =>
                    {
                        if (_lockSelectionChangedEvent)
                            return;

                        if (item.Selected)
                            this.SelectedItem = item;

                        _lockSelectionChangedEvent = true;
                        if (this.SelectionMode == ListViewItemsSelectionMode.Single && item.Selected)
                        {
                            foreach (var child in this.Children)
                                if (child is ISelectable && item != child)
                                    ((ISelectable)child).Selected = false;
                        }
                        _lockSelectionChangedEvent = false;
                        RaiseSelectionChanged();
                    };
                    base.OnVisualChildrenChanged(visualAdded, visualRemoved);
                }
                else
                    this.Children.Remove(visualAdded as UIElement);
            }
        }

        public ISelectable[] GetItems()
        {
            return this.Children.Cast<FrameworkElement>().Where(x => x is ISelectable).Select(x => (ISelectable)x).ToArray();
        }

        public ISelectable[] GetSelectedItems()
        {
            return GetItems().Where(x => x.Selected).ToArray();
        }

        public ListViewItemsSelectionMode SelectionMode
        {
            get
            {
                return (ListViewItemsSelectionMode)GetValue(SelectionModeProperty);
            }
            set
            {
                SetValue(SelectionModeProperty, value);
            }
        }

        public ISelectable SelectedItem
        {
            get
            {
                return (ISelectable)GetValue(SelectedItemProperty);
            }
            set
            {
                SetValue(SelectedItemProperty, value);
            }
        }

        private void RaiseSelectionChanged()
        {
            SelectionChanged?.Invoke(this, new ListItemsViewSelectionChangedEventArgs()
            {
                ListItemsView = this,
                SelectedItems = this.GetSelectedItems()
            });
        }

        public event RoutedEventHandler SelectionChanged;
    }
}