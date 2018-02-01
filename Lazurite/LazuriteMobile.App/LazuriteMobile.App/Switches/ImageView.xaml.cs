using Lazurite.MainDomain;

using Xamarin.Forms;

namespace LazuriteMobile.App.Switches
{
    public partial class ImageView : ContentView
    {
        public ImageView()
        {
            InitializeComponent();
        }

        public ImageView(ScenarioInfo scenario) : this()
        {
            BindingContext = new SwitchScenarioModel(scenario);
        }
    }
}
