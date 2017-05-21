using LazuriteUI.Icons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace LazuriteMobile.App.Controls
{
	public partial class IconView : Grid
	{
        public static readonly BindableProperty IconProperty;

        static IconView()
        {
            IconProperty = BindableProperty.Create(nameof(Icon), typeof(Icon), typeof(IconView), Icon.Power, BindingMode.OneWay, null,
                (sender, oldVal, newVal) => {
                    ((IconView)sender).iconControl.Source = ImageSource.FromStream(() =>
                        {
                            try
                            {
                                return Utils.GetIconData((Icon)newVal);
                            }
                            catch(Exception e)
                            {
                                var b = "asd";
                            }
                            return null;
                        }
                    );
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
