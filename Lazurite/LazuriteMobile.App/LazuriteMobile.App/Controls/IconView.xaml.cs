using LazuriteUI.Icons;

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
                            return Utils.GetIconData((Icon)newVal);
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
