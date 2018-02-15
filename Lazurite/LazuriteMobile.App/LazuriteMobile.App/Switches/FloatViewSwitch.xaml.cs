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

        private string _tempValue;//crutch#1
        private string _tempValue_current; //crutch#2
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
                slider.Value -= _iteration;
            else slider.Value += _iteration;
        }

        public FloatViewSwitch(SwitchScenarioModel model) : this()
        {
            BindingContext = model;
            //binding works incorrectly
            slider.Maximum = model.Max; //crutch
            slider.Minimum = model.Min; //crutch

            slider.Value = double.Parse(model.ScenarioValue);
            _tempValue = _tempValue_current = model.ScenarioValue;
            _iteration = (model.Max - model.Min) / 20;
            slider.ValueChanged += (o, e) => _tempValue = slider.Value.ToString();
            _tokenSource = SystemUtils.StartTimer(
                (token) => {
                    if (_tempValue != _tempValue_current)
                        model.ScenarioValue = _tempValue_current = _tempValue;
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