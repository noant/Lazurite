using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PyriteUI.Controls
{
    /// <summary>
    /// Логика взаимодействия для ListItemsView.xaml
    /// </summary>
    public partial class ListItemsView : StackPanel
    {
        public static readonly DependencyProperty SelectionModeProperty;

        static ListItemsView()
        {
            SelectionModeProperty = DependencyProperty.Register(nameof(SelectionMode), typeof(ListViewItemsSelectionMode), typeof(ListItemsView), new FrameworkPropertyMetadata() {
                PropertyChangedCallback = (o,e) =>
                {
                    var listItemsView = ((ListItemsView)o);
                    var mode = (ListViewItemsSelectionMode)e.NewValue;
                    foreach (var children in listItemsView.Children)
                    {
                        var itemView = children as ItemView;
                        if (itemView != null)
                        {
                            itemView.Selected = false;
                            itemView.Selectable = mode != ListViewItemsSelectionMode.None;
                        }
                    }
                }
            });
        }

        public ListItemsView()
        {
            InitializeComponent();
        }

        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            if (visualAdded == null && visualRemoved != null)
                base.OnVisualChildrenChanged(null, visualRemoved);
            else
            {
                var itemView = visualAdded as ItemView;
                if (itemView != null)
                {
                    itemView.Selectable = SelectionMode != ListViewItemsSelectionMode.None;
                    itemView.SelectionChanged += (o, e) =>
                    {
                        if (this.SelectionMode == ListViewItemsSelectionMode.Single && itemView.Selected)
                        {
                            foreach (var item in this.Children.Cast<Control>())
                            {
                                if (item is ItemView && item != itemView)
                                    ((ItemView)item).Selected = false;
                            }
                        }
                        RaiseSelectionChanged();
                    };
                    base.OnVisualChildrenChanged(itemView, visualRemoved);
                }
                else
                    this.Children.Remove(visualAdded as UIElement);
            }
        }

        public ItemView[] GetItems()
        {
            return this.Children.Cast<Control>().Where(x => x is ItemView).Select(x => (ItemView)x).ToArray();
        }

        public ItemView[] GetSelectedItems()
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

        private void RaiseSelectionChanged()
        {
            SelectionChanged?.Invoke(this, new ListItemsViewSelectionChangedEventArgs() {
                ListItemsView = this,
                SelectedItems = this.GetSelectedItems()                
            });
        }

        public event RoutedEventHandler SelectionChanged;
    }
}
