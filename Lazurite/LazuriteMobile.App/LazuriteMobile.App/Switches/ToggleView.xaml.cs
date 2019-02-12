using Lazurite.MainDomain;
using LazuriteMobile.App.Switches.Bases;
using LazuriteMobile.App.Switches.Bases.Converters;
using System;
using System.Threading;

namespace LazuriteMobile.App.Switches
{
    public partial class ToggleView : SwitchBase
    {
        public ToggleView()
        {
            InitializeComponent();
        }

        private ValueTypeStringToBool _converter = new ValueTypeStringToBool();
        private SwitchScenarioModel _model;

        public ToggleView(ScenarioInfo scenario) : this()
        {
            BindingContext = _model = new SwitchScenarioModel(scenario);
            var context = SynchronizationContext.Current;
            
            // Binding works incorrectly
            _model.PropertyChanged += (o, e) =>
            {
                if (e.PropertyName == nameof(SwitchScenarioModel.ScenarioValue))
                    context.Post((state) =>
                        itemView.Selected = (bool)_converter.Convert(_model.ScenarioValue, null, null, null),
                    null);
            };

            itemView.Selected = (bool)_converter.Convert(_model.ScenarioValue, null, null, null);
        }

        //binding works incorrectly
        private void itemView_SelectionChanged(object arg1, EventArgs arg2)
        {
            var model = (SwitchScenarioModel)BindingContext;
            var currValue = (bool)_converter.Convert(model.ScenarioValue, null, null, null);
            if (currValue != itemView.Selected)
                model.ScenarioValue = _converter.ConvertBack(itemView.Selected, null, null, null).ToString();
        }
    }
}
