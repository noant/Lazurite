using Lazurite.MainDomain;
using LazuriteMobile.App.Controls;
using LazuriteMobile.App.Switches.Bases;
using System;

namespace LazuriteMobile.App.Switches
{
    public partial class StatusView : SwitchBase
    {
        public StatusView()
        {
            InitializeComponent();
        }

        public StatusView(ScenarioInfo scenario) : this()
        {
            BindingContext = new SwitchScenarioModel(scenario);
            itemView.Click += ItemView_Click;
        }

        private void ItemView_Click(object sender, EventArgs e)
        {
            var model = (SwitchScenarioModel)BindingContext;
            var statusSwitch = new StatusViewSwitch(model);
            var dialog = new DialogView(statusSwitch);
            statusSwitch.StateChanged += (o, args) =>
            {
                if (args.Value == ItemView.ClickSource.CloseAnyway)
                    dialog.Close();
                if ((args.Value == ItemView.ClickSource.Tap && model.AcceptedValues.Length <= StatusViewSwitch.NotClosingItemsCount) ||
                    args.Value == ItemView.ClickSource.CloseAnyway)                                                                 // Если количество возможных значений меньше 14, 
                                                                                                                                    // то при выборе одного из значений закрываем окно.
                    dialog.Close();                                                                                                 // Число подобрано на основе того, сколько строчек вмещается на экран.
            };
            dialog.Show(Helper.GetLastParent(this));
        }
    }
}
