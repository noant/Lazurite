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
        }

        private void _changer_VolumeChanged(object sender, Lazurite.Shared.EventsArgs<int> args)
        {
            if (args.Value < 0)
                slider.Value -= _iteration;
            else slider.Value += _iteration;
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
            _tempValueToInstall = _tempValueToUpdate = model.ScenarioValue;

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
                _tempValueToUpdate = slider.Value.ToString();
                _scenarioValueChanged = false;
            };

            //crutch #3
            _tokenSource = SystemUtils.StartTimer(
                (token) => {
                    if (_tempValueToUpdate != _tempValueToInstall && !_scenarioValueChanged)
                    {
                        model.ScenarioValue = _tempValueToInstall = _tempValueToUpdate;
                    }
                    if (_tempValueToInstall != _tempValueToUpdate && _scenarioValueChanged)
                        Dispatcher.BeginInvoke(new Action(() => {
                            slider.Value = double.Parse(_tempValueToUpdate = _tempValueToInstall);
                        }));
                },
                () => FloatView_ValueUpdateInterval);
        }
    }
}
