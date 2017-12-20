using Lazurite.IOC;
using Lazurite.MainDomain;
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

        private IHardwareVolumeChanger _changer;
        private volatile string _tempValue;
        private CancellationTokenSource _tokenSource = new CancellationTokenSource();
        private double _iteration; 
        private ScenarioModel _model;

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

        public void Dispose()
        {
            if (_changer != null)
            {
                _changer.VolumeDown -= _changer_VolumeChanged;
                _changer.VolumeUp -= _changer_VolumeChanged;
            }
            _tokenSource.Cancel();
        }

        public FloatViewSwitch(ScenarioModel model): this()
        {
            this.DataContext = _model = model;
            this._tempValue = model.ScenarioValue;
            //crutch #1
            _model.PropertyChanged += (o, e) =>
            {
                if (e.PropertyName == nameof(_model.ScenarioValue))
                    slider.Dispatcher.BeginInvoke(
                        new Action(() => slider.Value = double.Parse(this._tempValue = model.ScenarioValue)));
            };
            this._iteration = (model.Max - model.Min) / 40;
            this.slider.Value = double.Parse(_tempValue ?? "0");

            //crutch #2
            this.slider.ValueChanged += (o, e) => _tempValue = slider.Value.ToString();

            //crutch #3
            TaskUtils.Start(() => {
                while (!_tokenSource.IsCancellationRequested)
                {
                    if (_tempValue != model.ScenarioValue)
                        model.ScenarioValue = _tempValue;
                    Thread.Sleep(FloatView_ValueUpdateInterval);
                }
            });
        }
    }
}
