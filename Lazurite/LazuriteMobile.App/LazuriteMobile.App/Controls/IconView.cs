using LazuriteUI.Icons;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace LazuriteMobile.App.Controls
{
    public class IconView: Image
    {
        public static readonly BindableProperty IconProperty;

        private static Dictionary<Icon, byte[]> _cache = new Dictionary<Icon, byte[]>();

        static IconView()
        {
            IconProperty = BindableProperty.Create(nameof(Icon), typeof(Icon), typeof(IconView), Icon.Power, BindingMode.OneWay, null,
                (sender, oldVal, newVal) => {
                    var view = sender as IconView;
                    var icon = (Icon)newVal;
                    var prev = (Icon)oldVal;
                    if (!_cache.ContainsKey(icon))
                        _cache.Add(icon, LazuriteUI.Icons.Utils.GetIconDataBytes(icon));
                    if (icon != prev)
                        view.OnPropertyChanged(nameof(view.Source));
                });
        }

        public IconView() : base()
        {
            Source = ImageSource.FromStream(() => new MemoryStream(_cache[Icon]));
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
