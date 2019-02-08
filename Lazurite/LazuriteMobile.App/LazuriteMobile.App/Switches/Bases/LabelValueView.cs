using Xamarin.Forms;

namespace LazuriteMobile.App.Switches.Bases
{
    public class LabelValueView : Label
    {
        public LabelValueView()
        {
            InitializeComponent();
        }

        public void InitializeComponent()
        {
            TextColor = Controls.Visual.Current.SwitchForeground;
            VerticalOptions = new LayoutOptions(LayoutAlignment.End, true);
            HorizontalOptions = new LayoutOptions(LayoutAlignment.Center, true);
            InputTransparent = true;
            FontSize = 11;
            FontFamily = Controls.Visual.Current.FontFamily;
        }
    }
}
