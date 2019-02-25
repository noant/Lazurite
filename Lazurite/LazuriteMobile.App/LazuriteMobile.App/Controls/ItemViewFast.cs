using Lazurite.Shared;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace LazuriteMobile.App.Controls
{
    public class ItemViewFast : Grid, ISelectable
    {
        public const int CharWidth = 9;

        public static readonly BindableProperty SelectedProperty;
        public static readonly BindableProperty SelectableProperty;
        public static readonly BindableProperty StrokeVisibleProperty;
        public static readonly BindableProperty TextProperty;

        public event EventsHandler<object> SelectionChanged;

        public event EventsHandler<object> Click;

        static ItemViewFast()
        {
            SelectableProperty = BindableProperty.Create(nameof(SelectableProperty), typeof(bool), typeof(ItemViewFast), false, BindingMode.OneWay, null,
                (sender, oldVal, newVal) =>
                {
                    if (!(bool)newVal)
                    {
                        ((ItemViewFast)sender).Selected = false;
                    }
                });
            SelectedProperty = BindableProperty.Create(nameof(SelectedProperty), typeof(bool), typeof(ItemViewFast), false, BindingMode.OneWay, null,
                (sender, oldVal, newVal) =>
                {
                    ((ItemViewFast)sender).BackgroundColor = (bool)newVal ? Controls.Visual.Current.ItemSelection : Controls.Visual.Current.ItemBackground;
                    ((ItemViewFast)sender).RaiseSelectionChanged();
                });
            StrokeVisibleProperty = BindableProperty.Create(nameof(StrokeVisible), typeof(bool), typeof(ItemViewFast), false, BindingMode.OneWay, null,
                (sender, oldVal, newVal) =>
                {
                    var itemView = ((ItemViewFast)sender);
                    if ((bool)newVal)
                    {
                        itemView.ShowStroke();
                        itemView.StartWaitingAndStrokeActions();
                    }
                    else
                    {
                        itemView.HideStroke();
                    }
                });
            TextProperty = BindableProperty.Create(nameof(Text), typeof(string), typeof(ItemViewFast), "itemView", BindingMode.OneWay, null,
                 (sender, oldVal, newVal) =>
                 {
                     (sender as ItemViewFast).ApplyText();
                 });
        }

        private Grid line;
        private Button button;
        private Label label;
        private bool _lockClick = false;

        public ItemViewFast()
        {
            BackgroundColor = Controls.Visual.Current.ItemBackground;

            button = new Button();
            button.BorderWidth = 0;
            button.BorderColor = Color.Transparent;
            button.BackgroundColor = Color.Transparent;
            button.FontSize = Controls.Visual.Current.FontSize;
            button.TextColor = Controls.Visual.Current.ItemForeground;
            button.Clicked += Button_Clicked;

            var round = new Label();
            round.Text = "•";
            round.FontSize = 33;
            round.TextColor = Color.LightSlateGray;
            round.VerticalTextAlignment = TextAlignment.Center;
            round.HorizontalOptions = new LayoutOptions(LayoutAlignment.Start, false);
            round.VerticalOptions = new LayoutOptions(LayoutAlignment.Center, true);
            round.InputTransparent = true;
            round.Margin = new Thickness(8, -3, 0, 0);

            label = new CaptionView();
            label.VerticalTextAlignment = TextAlignment.Center;
            label.HorizontalOptions = new LayoutOptions(LayoutAlignment.Center, false);
            label.VerticalOptions = new LayoutOptions(LayoutAlignment.Center, false);
            label.InputTransparent = true;

            Children.Add(button);
            Children.Add(label);
            Children.Add(round);

            SizeChanged += (o, e) => ApplyText();
        }

        private void ApplyText()
        {
            if (Text != null && Text.Length != 0 && Width > 0 && Height > 0)
            {
                var textWidth = Text.Length * CharWidth;
                var txt = Text.Replace("\r", string.Empty).Replace("\n", string.Empty);
                if (textWidth > Width)
                {
                    label.Text = Text.Substring(0, (int)(Width / CharWidth) - 2) + "...";
                }
                else
                {
                    label.Text = Text;
                }
            }
        }

        private void ShowStroke()
        {
            if (line != null)
            {
                line.IsVisible = true;
            }
            else
            {
                line = new Grid();
                line.HorizontalOptions = new LayoutOptions(LayoutAlignment.Fill, false);
                line.VerticalOptions = new LayoutOptions(LayoutAlignment.End, false);
                line.HeightRequest = 4;
                line.BackgroundColor = Color.Purple;

                Children.Add(line);
            }
        }

        private void HideStroke()
        {
            if (line != null)
            {
                line.IsVisible = false;
            }
        }

        async private void StartWaitingAndStrokeActions()
        {
            await Task.Delay(1000);
            if (StrokeVisible && !Selected)
            {
                Selected = true;
                StrokeVisible = false;
                if (StrokeVisibilityClick)
                {
                    Click?.Invoke(this, new EventsArgs<object>(ItemView.ClickSource.UnderscoreWaiting));
                }
            }
        }

        async private void Button_Clicked(object sender, EventArgs e)
        {
            if (!_lockClick && button.IsEnabled)
            {
                _lockClick = true;
                var view = this;
                await view.ScaleTo(0.85, 50, Easing.CubicIn)
                    .ContinueWith((o) => view.ScaleTo(1, 50, Easing.CubicOut));
                _lockClick = false;
                if (Selectable)
                {
                    Selected = !Selected;
                }

                Click?.Invoke(this, new EventsArgs<object>(ItemView.ClickSource.Tap));
            }
        }

        private void RaiseSelectionChanged()
        {
            SelectionChanged?.Invoke(this, new EventsArgs<object>(this));
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

        public bool StrokeVisibilityClick { get; set; } = false;
    }
}