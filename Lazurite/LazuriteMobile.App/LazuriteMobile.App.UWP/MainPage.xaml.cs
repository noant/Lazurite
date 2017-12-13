namespace LazuriteMobile.App.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();

            LoadApplication(new LazuriteMobile.App.App());
        }
    }
}
