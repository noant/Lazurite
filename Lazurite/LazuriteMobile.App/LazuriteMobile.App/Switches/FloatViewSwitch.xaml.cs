using Lazurite.IOC;
using Lazurite.MainDomain;
using Lazurite.Utils;
using System.Threading;

using Xamarin.Forms;

namespace LazuriteMobile.App.Switches
{
    public partial class FloatViewSwitch : ContentView
    {
        private static readonly int FloatView_ValueUpdateInterval = GlobalSettings.Get(300);
        private static ISystemUtils SystemUtils = Singleton.Resolve<ISystemUtils>();

        private string _tempValue;
        private CancellationTokenSource _tokenSource = new CancellationTokenSource();
        private double _iteration;

        public FloatViewSwitch()
        {
            InitializeComponent();
        }

        public FloatViewSwitch(SwitchScenarioModel model) : this()
        {
            this.BindingContext = model;
            //binding works incorrectly
            slider.Maximum = model.Max; //crutch
            slider.Minimum = model.Min; //crutch
            slider.Value = double.Parse(model.ScenarioValue);

            this.slider.ValueChanged += (o, e) =>
            {
                _tempValue = slider.Value.ToString();
            };

            TaskUtils.Start(() => {
                while (!_tokenSource.IsCancellationRequested)
                {
                    if (_tempValue != model.ScenarioValue)
                        model.ScenarioValue = _tempValue;
                    SystemUtils.Sleep(FloatView_ValueUpdateInterval, CancellationToken.None);
                }
            });
        }
    }
}
