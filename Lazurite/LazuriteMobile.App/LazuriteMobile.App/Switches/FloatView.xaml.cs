using Lazurite.MainDomain;
using LazuriteMobile.App.Controls;
using LazuriteMobile.App.Switches.Bases;
using System;
using System.Threading;

namespace LazuriteMobile.App.Switches
{
    public partial class FloatView : SwitchBase
    {
        private SynchronizationContext _currentContext = SynchronizationContext.Current;

        private SwitchScenarioModel _model;
        public FloatView()
        {
            InitializeComponent();
        }
        
        public FloatView(ScenarioInfo scenario) : this()
        {
            _model = new SwitchScenarioModel(scenario);
            BindingContext = _model;
            itemView.Click += itemView_Click;
        }

        private void itemView_Click(object sender, EventArgs e)
        {
            var controlSlider = new FloatViewSliderSwitch(_model);
            var dialogSlider = new DialogView(controlSlider);
            controlSlider.ManualInputActivate += (o1, e1) => {
                _currentContext.Post((s) => {
                    dialogSlider.Close();
                    var controlManual = new FloatViewManualSwitch(_model);
                    var dialogManual = new DialogView(controlManual);
                    controlManual.ApplyClicked += (o2, e2) =>
                    {
                        _model.ScenarioValue = e2.Value;
                        dialogManual.Close();
                    };
                    dialogManual.Show(Helper.GetLastParent(this));
                },
                null);
            };
            dialogSlider.Show(Helper.GetLastParent(this));
        }
    }
}
