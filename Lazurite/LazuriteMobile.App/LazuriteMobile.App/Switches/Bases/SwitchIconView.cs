using LazuriteMobile.App.Controls;
using Xamarin.Forms;

namespace LazuriteMobile.App.Switches.Bases
{
    public class SwitchIconView : Controls.IconView
    {
        public SwitchIconView()
        {
            InitializeComponent();
        }

        public void InitializeComponent()
        {
            WidthRequest = 45;
            HeightRequest = 45;
            VerticalOptions = new LayoutOptions(LayoutAlignment.Center, true);
            HorizontalOptions = new LayoutOptions(LayoutAlignment.Center, true);
            InputTransparent = true;
            Foreground = Visual.Current.SwitchIconColor;
        }
    }
}
