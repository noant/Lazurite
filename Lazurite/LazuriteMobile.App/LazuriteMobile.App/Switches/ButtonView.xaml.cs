using Lazurite.MainDomain;
using LazuriteMobile.App.Switches.Bases;
using System;

namespace LazuriteMobile.App.Switches
{
    public partial class ButtonView : SwitchBase
    {
        private SwitchScenarioModel _model;
        public ButtonView()
        {
            InitializeComponent();
        }
        
        public ButtonView(ScenarioInfo scenario) : this()
        {
            _model = new SwitchScenarioModel(scenario);
            BindingContext = _model;
            itemView.Click += ItemView_Click;
        }

        private void ItemView_Click(object arg1, EventArgs arg2)
        {
            _model.ScenarioValue = string.Empty;
        }
    }
}