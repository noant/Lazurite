using System;
using System.Linq;
using Xamarin.Forms;

namespace LazuriteMobile.App.Controls
{
    public class ListItemsView: StackLayout
    {
        public static readonly BindableProperty SelectionModeProperty;

        static ListItemsView()
        {
            SelectionModeProperty = BindableProperty.Create(nameof(SelectionMode), typeof(ListViewItemsSelectionMode), typeof(ListItemsView), ListViewItemsSelectionMode.None, BindingMode.TwoWay, null,
                (o, oldVal, newVal) =>
                {
                    var listItemsView = ((ListItemsView)o);
                    var mode = (ListViewItemsSelectionMode)newVal;
                    foreach (var children in listItemsView.Children)
                    {
                        var item = children as ISelectable;
                        if (item != null)
                        {
                            item.Selected = false;
                            item.Selectable = mode != ListViewItemsSelectionMode.None;
                        }
                    }
            });
        }

        public ListItemsView()
        {
            Orientation = StackOrientation.Vertical;
            Spacing = 3;
        }

        protected override void OnChildAdded(Element child)
        {
            var item = child as ISelectable;
            if (item != null)
            {
                var c = ((View)child);
                c.HorizontalOptions = new LayoutOptions(LayoutAlignment.Fill, true);
                c.WidthRequest = Width;
                item.Selectable = SelectionMode != ListViewItemsSelectionMode.None;
                item.SelectionChanged += (o, e) =>
                {
                    if (SelectionMode == ListViewItemsSelectionMode.Single && item.Selected)
                    {
                        foreach (var view in Children)
                        {
                            var selectable = view as ISelectable;
                            if (selectable != null && selectable != child)
                                selectable.Selected = false;
                        }
                    }
                    RaiseSelectionChanged();
                };
                base.OnChildAdded(child);
            }
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

        public ISelectable[] GetItems()
        {
            return Children.Where(x => x is ISelectable).Select(x => (ISelectable)x).ToArray();
        }

        public ISelectable[] GetSelectedItems()
        {
            return GetItems().Where(x => x.Selected).ToArray();
        }

        public event Action<object, ListItemsViewSelectionChangedEventArgs> SelectionChanged;

        private void RaiseSelectionChanged()
        {
            SelectionChanged?.Invoke(this, new ListItemsViewSelectionChangedEventArgs()
            {
                ListItemsView = this,
                SelectedItems = GetSelectedItems()
            });
        }
    }
}
