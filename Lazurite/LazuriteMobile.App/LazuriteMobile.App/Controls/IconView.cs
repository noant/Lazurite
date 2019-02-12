using LazuriteUI.Icons;
using System.IO;
using Xamarin.Forms;

namespace LazuriteMobile.App.Controls
{
    // Many thanks to https://github.com/andreinitescu
    public class IconView: View
    {
        public static readonly BindableProperty IconProperty;

        public static readonly BindableProperty ForegroundProperty = BindableProperty.Create(nameof(Foreground), typeof(Color), typeof(IconView), Visual.Current.StandardIconColor);

        public static readonly BindableProperty SourceProperty = BindableProperty.Create(nameof(Source), typeof(Stream), typeof(IconView), null);

        static IconView()
        {
            IconProperty = BindableProperty.Create(nameof(Icon), typeof(Icon), typeof(IconView), Icon.Power, BindingMode.OneWay, null,
                (sender, oldVal, newVal) => {
                    var view = sender as IconView;
                    var icon = (Icon)newVal;
                    var prev = (oldVal is Icon p) ? p : Icon._None;
                    view.Source = LazuriteUI.Icons.Utils.GetIconData(icon);
                    view.OnPropertyChanged(nameof(view.Source));
                });
        }

        public IconView()
        {
            // Do nothing
        }
        
        public Icon Icon
        {
            get => (Icon)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public Stream Source
        {
            get => (Stream)GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        public Color Foreground
        {
            get => (Color)GetValue(ForegroundProperty);
            set => SetValue(ForegroundProperty, value);
        }
    }
}
