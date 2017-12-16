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
            this.Orientation = StackOrientation.Vertical;
            this.Spacing = 3;
        }

        protected override void OnChildAdded(Element child)
        {
            var item = child as ISelectable;
            if (item != null)
            {
                ((View)child).HorizontalOptions = new LayoutOptions(LayoutAlignment.Fill, true);
                ((View)child).WidthRequest = this.Width;
                item.Selectable = SelectionMode != ListViewItemsSelectionMode.None;
                item.SelectionChanged += (o, e) =>
                {
                    if (this.SelectionMode == ListViewItemsSelectionMode.Single && item.Selected)
                    {
                        foreach (var view in this.Children)
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
            return this.Children.Where(x => x is ISelectable).Select(x => (ISelectable)x).ToArray();
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
                SelectedItems = this.GetSelectedItems()
            });
        }
    }
}
