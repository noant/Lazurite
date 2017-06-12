using Lazurite.MainDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace LazuriteMobile.App.Switches
{
    public partial class ToggleView : ContentView
    {
        public ToggleView()
        {
            InitializeComponent();
        }

        private ValueTypeStringToBool _converter = new ValueTypeStringToBool();

        public ToggleView(ScenarioInfo scenario) : this()
        {
            var model = new ScenarioModel(scenario);
            this.BindingContext = model;
            var context = SynchronizationContext.Current;
            //binding works incorrectly
            model.PropertyChanged += (o, e) =>
            {
                if (e.PropertyName == nameof(ScenarioModel.ScenarioValue))
                    context.Post((state) => {
                        itemView.Selected = (bool)_converter.Convert(model.ScenarioValue, null, null, null);
                    }, null);
            };
            itemView.Selected = (bool)_converter.Convert(model.ScenarioValue, null, null, null);
        }

        //binding works incorrectly
        private void itemView_SelectionChanged(object arg1, EventArgs arg2)
        {
            var model = ((ScenarioModel)this.BindingContext);
            var currValue = (bool)_converter.Convert(model.ScenarioValue, null, null, null);
            if (currValue != itemView.Selected)
                model.ScenarioValue = _converter.ConvertBack(itemView.Selected, null, null, null).ToString();
        }
    }
}
