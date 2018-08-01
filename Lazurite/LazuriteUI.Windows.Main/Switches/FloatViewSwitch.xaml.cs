using Lazurite.IOC;
using Lazurite.MainDomain;
using Lazurite.Scenarios.ScenarioTypes;
using Lazurite.Utils;
using System;
using System.Threading;
using System.Windows.Controls;

namespace LazuriteUI.Windows.Main.Switches
{
    /// <summary>
    /// Логика взаимодействия для FloatView.xaml
    /// </summary>
    public partial class FloatViewSwitch : UserControl, IDisposable
    {
        private static readonly int FloatView_ValueUpdateInterval = GlobalSettings.Get(300);
        private static readonly ISystemUtils SystemUtils = Singleton.Resolve<ISystemUtils>();

        private IHardwareVolumeChanger _changer;
        private volatile string _tempValueToInstall;
        private volatile string _tempValueToUpdate;
        private CancellationTokenSource _tokenSource;
        private double _iteration;
        private int _round = 1;
        private ScenarioModel _model;
        private bool _scenarioValueChanged;

        public FloatViewSwitch()
        {
            InitializeComponent();
            if (Singleton.Any<IHardwareVolumeChanger>())
            {
                _changer = Singleton.Resolve<IHardwareVolumeChanger>();
                _changer.VolumeDown += _changer_VolumeChanged;
                _changer.VolumeUp += _changer_VolumeChanged;
            }

            btInput.Click += BtInput_Click;
        }

        private void BtInput_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            InfoViewSwitch.Show(
                (val) => {
                    if (double.TryParse(val, out double value))
                        slider.Value = value;
                },
                true, 
                _model.Min, 
                _model.Max);
        }

        private void _changer_VolumeChanged(object sender, Lazurite.Shared.EventsArgs<int> args)
        {
            var iteration = _iteration * args.Value;
            if (slider.Value + iteration > slider.Maximum)
                slider.Value = slider.Maximum;
            else if (slider.Value + iteration < slider.Minimum)
                slider.Value = slider.Minimum;
            else
                slider.Value += iteration;
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

        public FloatViewSwitch(ScenarioModel model): this()
        {
            DataContext = _model = model;

            var diff = model.Max - model.Min;
            _round = (int)(2 / diff);

            _tempValueToInstall = _tempValueToUpdate = model.ScenarioValue;
            lblCaption.Content = _model.ScenarioValueWithUnit; //crutch #0

            //crutch #1
            _model.PropertyChanged += (o, e) =>
            {
                if (e.PropertyName == nameof(_model.ScenarioValue))
                {
                    if (model.ScenarioValue != _tempValueToInstall)
                    {
                        _tempValueToInstall = model.ScenarioValue;
                        _scenarioValueChanged = true;
                    }
                }
            };

            _iteration = (model.Max - model.Min) / 40;
            slider.Value = double.Parse(_tempValueToInstall ?? "0");

            //crutch #2
            slider.ValueChanged += (o, e) =>
            {
                var value = slider.Value;
                value = Math.Round(value, _round);

                _tempValueToUpdate = value.ToString();
                _scenarioValueChanged = false;

                lblCaption.Content = _tempValueToUpdate + _model.Unit; //crutch #3
            };

            //crutch #4
            _tokenSource = SystemUtils.StartTimer(
                (token) => {
                    if (_tempValueToUpdate != _tempValueToInstall && !_scenarioValueChanged)
                        model.ScenarioValue = _tempValueToInstall = _tempValueToUpdate;
                    if (_tempValueToInstall != _tempValueToUpdate && _scenarioValueChanged)
                        Dispatcher.BeginInvoke(new Action(() => {
                            slider.Value = double.Parse(_tempValueToUpdate = _tempValueToInstall);
                        }));
                },
                () => FloatView_ValueUpdateInterval);
        }
    }
}
