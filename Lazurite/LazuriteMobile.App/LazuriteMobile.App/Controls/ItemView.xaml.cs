using Lazurite.Shared;
using LazuriteUI.Icons;
using System;
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
        public static readonly BindableProperty AnimateViewProperty;
        public static readonly BindableProperty StrokeVisibleProperty;
        public static readonly BindableProperty SelectionColorProperty;

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
                    ((ItemView)sender).label.Text = (string)newVal;
                });
            SelectableProperty = BindableProperty.Create(nameof(SelectableProperty), typeof(bool), typeof(ItemView), false, BindingMode.OneWay, null,
                (sender, oldVal, newVal) =>
                {
                    if (!(bool)newVal)
                        ((ItemView)sender).Selected = false;
                });
            SelectedProperty = BindableProperty.Create(nameof(SelectedProperty), typeof(bool), typeof(ItemView), false, BindingMode.OneWay, null,
                (sender, oldVal, newVal) =>
                {
                    ((ItemView)sender).backGrid.Opacity = (bool)newVal ? 1 : 0; //crutch; IsVisibile not works (sic!);
                    ((ItemView)sender).RaiseSelectionChanged();
                });
            StrokeVisibleProperty = BindableProperty.Create(nameof(StrokeVisible), typeof(bool), typeof(ItemView), false, BindingMode.OneWay, null,
                (sender, oldVal, newVal) =>
                {
                    ((ItemView)sender).strokeGrid.Opacity = (bool)newVal ? 1 : 0; //crutch; IsVisibile not works (sic!)
                    if ((bool)newVal)
                        ((ItemView)sender).StartWaitingAndStrokeActions();
                });
            SelectionColorProperty = BindableProperty.Create(nameof(SelectionColor), typeof(Color), typeof(ItemView), Color.SteelBlue, BindingMode.OneWay, null,
                (sender, oldVal, newVal) =>
                {
                    ((ItemView)sender).backGrid.BackgroundColor = (Color)newVal;
                });
            AnimateViewProperty = BindableProperty.Create(nameof(AnimateView), typeof(View), typeof(ItemView), null, BindingMode.OneWay);
        }

        public ItemView()
		{
			InitializeComponent();
            PropertyChanged += (o, e) => {
                if (e.PropertyName == nameof(IsEnabled))
                {
                    button.IsVisible = IsEnabled;
                    backGrid.IsVisible = IsEnabled && Selected;
                }
            };
        }

        public bool StrokeVisible
        {
            get
            {
                return (bool)GetValue(StrokeVisibleProperty);
            }
            set
            {
                SetValue(StrokeVisibleProperty, value);
            }
        }

        public Color SelectionColor
        {
            get
            {
                return (Color)GetValue(SelectionColorProperty);
            }
            set
            {
                SetValue(SelectionColorProperty, value);
            }
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

        public View AnimateView
        {
            get
            {
                return (View)GetValue(AnimateViewProperty);
            }
            set
            {
                SetValue(AnimateViewProperty, value);
            }
        }

        async private void Button_Clicked(object sender, EventArgs e)
        {
            if (button.IsEnabled)
            {
                var view = AnimateView ?? this;
                await view.ScaleTo(0.85, 50, Easing.Linear).ContinueWith((o) =>
                    view.ScaleTo(1, 50, Easing.Linear)
                        .ContinueWith(o1 => Clicked?.Invoke(this, new EventsArgs<object>(this)))
                );
                if (Selectable)
                    Selected = !Selected;
                Click?.Invoke(this, new EventsArgs<object>(this));
            }
        }

        async private void StartWaitingAndStrokeActions()
        {
            await Task.Delay(1000);
            if (StrokeVisible && !Selected)
                Selected = true;
        }

        private void RaiseSelectionChanged()
        {
            SelectionChanged?.Invoke(this, new EventsArgs<object>(this));
        }

        public event EventsHandler<object> Click;
        public event EventsHandler<object> Clicked;
        public event EventsHandler<object> SelectionChanged;
    }
}
