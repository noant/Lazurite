using LazuriteMobile.App.Controls;
using Xamarin.Forms;

namespace LazuriteMobile.App.Switches.Bases
{
    public class SwitchCaptionView: CaptionView
    {
        public override void InitializeComponent()
        {
            base.InitializeComponent();
            TextColor = Visual.Current.SwitchForeground;
            VerticalOptions = new LayoutOptions(LayoutAlignment.End, true);
            HorizontalOptions = new LayoutOptions(LayoutAlignment.Center, true);
        }
    }
}
