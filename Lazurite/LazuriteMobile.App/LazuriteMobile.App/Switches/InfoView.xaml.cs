using Lazurite.MainDomain;
using LazuriteMobile.App.Controls;
using LazuriteMobile.App.Switches.Bases;

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

            itemView.Click += (o, e) =>
            {
                var parent = DialogView.GetDialogHost(this);
                InfoViewSwitch.Show(model, parent);
            };
        }
    }
}