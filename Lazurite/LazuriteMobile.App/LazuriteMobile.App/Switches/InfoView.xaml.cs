using Lazurite.MainDomain;
using LazuriteMobile.App.Switches.Bases;
using Xamarin.Forms;

namespace LazuriteMobile.App.Switches
{
    public partial class InfoView : SwitchBase
    {
        public InfoView()
        {
            InitializeComponent();
        }

        public InfoView(ScenarioInfo scenario) : this()
        {
            var model = new SwitchScenarioModel(scenario);
            BindingContext = model;

            itemView.Click += (o, e) => {
                var parent = Helper.GetLastParent(this);
                InfoViewSwitch.Show(model, parent);
            };
        }
    }
}
