using Lazurite.IOC;
using Lazurite.MainDomain;
using Lazurite.Utils;
using System;
using System.Threading;

using Xamarin.Forms;

namespace LazuriteMobile.App.Switches
{
    public partial class FloatViewSwitch : ContentView, IDisposable
    {
        private static readonly int FloatView_ValueUpdateInterval = GlobalSettings.Get(300);
        private static ISystemUtils SystemUtils = Singleton.Resolve<ISystemUtils>();

        private string _tempValue;
        private double _iteration;
        private CancellationTokenSource _tokenSource = new CancellationTokenSource();
        private IHardwareVolumeChanger _changer;

        public FloatViewSwitch()
        {
            InitializeComponent();
            if (Singleton.Any<IHardwareVolumeChanger>())
            {
                _changer = Singleton.Resolve<IHardwareVolumeChanger>();
                _changer.VolumeDown += _changer_VolumeChanged;
                _changer.VolumeUp += _changer_VolumeChanged;
            }
        }

        private void _changer_VolumeChanged(object sender, Lazurite.Shared.EventsArgs<int> args)
        {
            if (args.Value < 0)
                this.slider.Value -= _iteration;
            else this.slider.Value += _iteration;
        }

        public FloatViewSwitch(SwitchScenarioModel model) : this()
        {
            this.BindingContext = model;
            //binding works incorrectly
            slider.Maximum = model.Max; //crutch
            slider.Minimum = model.Min; //crutch

            slider.Value = double.Parse(model.ScenarioValue);
            this._tempValue = model.ScenarioValue;
            this._iteration = (model.Max - model.Min) / 20;
            this.slider.ValueChanged += (o, e) =>
            {
                _tempValue = slider.Value.ToString();
            };

            _tokenSource = SystemUtils.StartTimer(
                (token) => {
                    if (_tempValue != model.ScenarioValue)
                        model.ScenarioValue = _tempValue;
                }, 
                () => FloatView_ValueUpdateInterval);            
        }

        public void Dispose()
        {
            if (_changer != null)
            {
                _changer.VolumeDown -= _changer_VolumeChanged;
                _changer.VolumeUp -= _changer_VolumeChanged;
            }
            _tokenSource?.Cancel();
        }
    }
}
