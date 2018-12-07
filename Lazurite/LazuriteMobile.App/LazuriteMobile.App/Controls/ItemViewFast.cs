using Lazurite.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace LazuriteMobile.App.Controls
{
    public class ItemViewFast: Grid, ISelectable
    {
        public static readonly BindableProperty SelectedProperty;
        public static readonly BindableProperty SelectableProperty;
        public static readonly BindableProperty StrokeVisibleProperty;
        public static readonly BindableProperty TextProperty;

        public event EventsHandler<object> SelectionChanged;

        static ItemViewFast()
        {
            SelectableProperty = BindableProperty.Create(nameof(SelectableProperty), typeof(bool), typeof(ItemViewFast), false, BindingMode.OneWay, null,
                (sender, oldVal, newVal) =>
                {
                    if (!(bool)newVal)
                        ((ItemViewFast)sender).Selected = false;
                });
            SelectedProperty = BindableProperty.Create(nameof(SelectedProperty), typeof(bool), typeof(ItemViewFast), false, BindingMode.OneWay, null,
                (sender, oldVal, newVal) =>
                {
                    ((ItemViewFast)sender).BackgroundColor = (bool)newVal ? Visual.ItemSelection : Visual.ItemBackground;
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
                        itemView.HideStroke();
                });
            TextProperty = BindableProperty.Create(nameof(Text), typeof(string), typeof(ItemViewFast), "itemView", BindingMode.OneWay, null,
                 (sender, oldVal, newVal) => {
                     (sender as ItemViewFast).button.Text = (string)newVal;
                 });
        }

        private Grid line;
        private Button button;

        public ItemViewFast()
        {
            BackgroundColor = Visual.ItemBackground;

            button = new Button();
            button.BorderWidth = 0;
            button.BorderColor = Color.Transparent;
            button.BackgroundColor = Color.Transparent;
            button.FontSize = Visual.FontSize;
            button.TextColor = Visual.Foreground;

            Children.Add(button);
        }
        
        private void ShowStroke()
        {
            if (line != null)
                line.IsVisible = true;
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
                line.IsVisible = false;
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
    }
}
