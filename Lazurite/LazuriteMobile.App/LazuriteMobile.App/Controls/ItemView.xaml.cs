using LazuriteUI.Icons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace LazuriteMobile.App.Controls
{
	public partial class ItemView : Grid, ISelectable
    {
        public static readonly BindableProperty IconVisibilityProperty;
        public static readonly BindableProperty IconProperty;
        public static readonly BindableProperty TextProperty;
        public static readonly BindableProperty SelectedProperty;
        public static readonly BindableProperty SelectableProperty;

        static ItemView()
        {
            IconVisibilityProperty = BindableProperty.Create(nameof(IconVisibility), typeof(bool), typeof(ItemView), true, BindingMode.OneWay, null,
                (sender, oldVal, newVal) =>
                {
                    ((ItemView)sender).iconView.IsVisible = (bool)newVal;
                });
            IconProperty = BindableProperty.Create(nameof(Icon), typeof(Icon), typeof(ItemView), Icon.Power, BindingMode.OneWay, null,
                (sender, oldVal, newVal) =>
                {
                    ((ItemView)sender).iconView.Icon = (Icon)newVal;
                });
            TextProperty = BindableProperty.Create(nameof(Text), typeof(string), typeof(ItemView), "itemView", BindingMode.OneWay, null,
                (sender, oldVal, newVal) => {
                    ((ItemView)sender).button.Text = (string)newVal;
                });
            SelectableProperty = BindableProperty.Create(nameof(SelectableProperty), typeof(bool), typeof(ItemView), false, BindingMode.TwoWay, null,
                (sender, oldVal, newVal) =>
                {
                    if (!(bool)newVal)
                        ((ItemView)sender).Selected = false;
                });
            SelectedProperty = BindableProperty.Create(nameof(SelectedProperty), typeof(bool), typeof(ItemView), false, BindingMode.TwoWay, null,
                (sender, oldVal, newVal) =>
                {
                    ((ItemView)sender).backGrid.IsVisible = (bool)newVal;
                    ((ItemView)sender).RaiseSelectionChanged();
                });
        }

        public ItemView()
		{
			InitializeComponent();
		}

        public bool IconVisibility
        {
            get
            {
                return (bool)GetValue(IconVisibilityProperty);
            }
            set
            {
                SetValue(IconVisibilityProperty, value);
            }
        }

        public Icon Icon
        {
            get
            {
                return (Icon)GetValue(IconProperty);
            }
            set
            {
                SetValue(IconProperty, value);
            }
        }

        public string Text
        {
            get
            {
                return (string)GetValue(TextProperty);
            }
            set
            {
                SetValue(TextProperty, value);
            }
        }

        public bool Selectable
        {
            get
            {
                return (bool)GetValue(SelectableProperty);
            }
            set
            {
                SetValue(SelectableProperty, value);
            }
        }

        public bool Selected
        {
            get
            {
                return (bool)GetValue(SelectedProperty);
            }
            set
            {
                SetValue(SelectedProperty, value);
            }
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            if (this.Selectable)
                this.Selected = !this.Selected;
            Click?.Invoke(this, new EventArgs());
        }

        private void RaiseSelectionChanged()
        {
            SelectionChanged?.Invoke(this, new EventArgs());
        }

        public event Action<object, EventArgs> Click;
        public event Action<object, EventArgs> SelectionChanged;
    }
}
