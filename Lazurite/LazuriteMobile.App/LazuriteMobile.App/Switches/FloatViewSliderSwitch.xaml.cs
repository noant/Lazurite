using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.IOC;
using Lazurite.MainDomain;
using Lazurite.Shared;
using Lazurite.Utils;
using System;

using Xamarin.Forms;

namespace LazuriteMobile.App.Switches
{
    public partial class FloatViewSliderSwitch : Grid, IDisposable
    {
        private static readonly int FloatView_ValueUpdateInterval = GlobalSettings.Get(300);
        private static ISystemUtils SystemUtils = Singleton.Resolve<ISystemUtils>();

        private string _tempValue; //crutch#1
        private string _tempValue_current; //crutch#2
        private double _iteration;
        private SafeCancellationToken _tokenSource = new SafeCancellationToken();
        private IHardwareVolumeChanger _changer;
        private int _round;
        private SwitchScenarioModel _model;

        public FloatViewSliderSwitch()
        {
            InitializeComponent();
            if (Singleton.Any<IHardwareVolumeChanger>())
            {
                _changer = Singleton.Resolve<IHardwareVolumeChanger>();
                _changer.VolumeDown += _changer_VolumeChanged;
                _changer.VolumeUp += _changer_VolumeChanged;
            }

            btInput.Clicked += BtInput_Clicked;
            btOn.Clicked += (o, e) => SetMax();
            btMiddle.Clicked += (o, e) => SetMiddle();
            btOff.Clicked += (o, e) => SetMin();
        }

        private void SetMin()
        {
            var valueType = _model.Scenario.ValueType as FloatValueType;
            SetValueFromButton(valueType.Min);
        }

        private void SetMax()
        {
            var valueType = _model.Scenario.ValueType as FloatValueType;
            SetValueFromButton(valueType.Max);
        }

        private void SetMiddle()
        {
            var valueType = _model.Scenario.ValueType as FloatValueType;
            SetValueFromButton(valueType.Min + (valueType.Max - valueType.Min) / 2);
        }

        private void SetValueFromButton(double value)
        {
            _model.ScenarioValue = value.ToString();
            NeedClose?.Invoke(this, EventArgs.Empty);
        }

        private void BtInput_Clicked(object sender, Lazurite.Shared.EventsArgs<object> args)
        {
            ManualInputActivate?.Invoke(this, EventArgs.Empty);
        }

        private void _changer_VolumeChanged(object sender, Lazurite.Shared.EventsArgs<int> args)
        {
            var iteration = _iteration * args.Value;
            if (slider.Value + iteration > slider.Maximum)
            {
                slider.Value = slider.Maximum;
            }
            else if (slider.Value + iteration < slider.Minimum)
            {
                slider.Value = slider.Minimum;
            }
            else
            {
                slider.Value += iteration;
            }
        }

        public FloatViewSliderSwitch(SwitchScenarioModel model) : this()
        {
            BindingContext = _model = model;

            var diff = model.Max - model.Min;
            _round = (int)(2 / diff);

            //binding works incorrectly
            slider.Maximum = model.Max; //crutch
            slider.Minimum = model.Min; //crutch

            slider.Value = double.Parse(model.ScenarioValue);
            _tempValue = _tempValue_current = model.ScenarioValue;
            _iteration = (model.Max - model.Min) / 20;
            slider.ValueChanged += (o, e) =>
            {
                var value = slider.Value;
                value = Math.Round(value, _round);
                _tempValue = value.ToString();
            };
            _tokenSource = SystemUtils.StartTimer(
                (token) =>
                {
                    if (_tempValue != _tempValue_current)
                    {
                        model.ScenarioValue = _tempValue_current = _tempValue;
                    }
                },
                () => FloatView_ValueUpdateInterval);
            SizeChanged += (o, e) =>
            {
                HeightRequest = Width * 2;
            };
        }

        public void Dispose()
        {
            if (_changer != null)
            {
                _changer.VolumeDown -= _changer_VolumeChanged;
                _changer.VolumeUp -= _changer_VolumeChanged;
            }
            _tokenSource?.Cancel();
            _tokenSource = null;
        }

        public event EventHandler ManualInputActivate;

        public event EventHandler NeedClose;
    }
}