using Lazurite.MainDomain;

using Xamarin.Forms;

namespace LazuriteMobile.App.Switches
{
    public partial class InfoView : ContentView
    {
        public InfoView()
        {
            InitializeComponent();
        }

        public InfoView(ScenarioInfo scenario) : this()
        {
            this.BindingContext = new SwitchScenarioModel(scenario);
        }
    }
}
