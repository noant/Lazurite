using LazuriteUI.Icons;
using System.Collections.Generic;
using System.Reflection;
using Xamarin.Forms;

namespace LazuriteMobile.App.Controls
{
    public partial class IconView : Grid
	{
        public static readonly BindableProperty IconProperty;

        private static Dictionary<Icon, ImageSource> _cache = new Dictionary<Icon, ImageSource>();

        static IconView()
        {
            IconProperty = BindableProperty.Create(nameof(Icon), typeof(Icon), typeof(IconView), Icon.Power, BindingMode.OneWay, null,
                (sender, oldVal, newVal) => {
                    var icon = (Icon)newVal;
                    ImageSource imageSource = null;
                    if (!_cache.ContainsKey(icon))
                        _cache.Add(icon, imageSource = ImageSource.FromResource(LazuriteUI.Icons.Utils.GetIconResourceName(icon), typeof(Icon).GetTypeInfo().Assembly));
                    else imageSource = _cache[icon];
                    ((IconView)sender).iconControl.Source = imageSource;
                });
        }

        public IconView ()
		{
			InitializeComponent ();
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
	}
}
