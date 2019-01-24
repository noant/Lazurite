using Xamarin.Forms;

namespace LazuriteMobile.App.Switches.Bases
{
    public class LabelCaptionView: Label
    {
        public LabelCaptionView()
        {
            InitializeComponent();
        }

        public void InitializeComponent()
        {
            TextColor = Controls.Visual.Foreground;
            VerticalOptions = new LayoutOptions(LayoutAlignment.End, true);
            HorizontalOptions = new LayoutOptions(LayoutAlignment.Center, true);
            InputTransparent = true;
            FontSize = 11;
            FontFamily = Controls.Visual.FontFamily;
        }
    }
}
