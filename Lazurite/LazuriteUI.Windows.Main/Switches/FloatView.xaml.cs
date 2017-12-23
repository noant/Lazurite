using Lazurite.MainDomain;
using Lazurite.Shared;
using Lazurite.Utils;
using LazuriteUI.Windows.Controls;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LazuriteUI.Windows.Main.Switches
{
    /// <summary>
    /// Логика взаимодействия для ToggleView.xaml
    /// </summary>
    public partial class FloatView : UserControl, IHardwareVolumeChanger
    {
        private static readonly int FloatView_SmoothChangeValueInterval = GlobalSettings.Get(300);

        private Thread _smoothChangeValueThread; //crutch
        private string SmoothChangeValueToSet
        {
            get
            {
                return _model.SmoothChangeValue;
            }
            set
            {
                _model.SmoothChangeValue = value;
            }
        } //crutch

        private ScenarioModel _model;
        private IHardwareVolumeChanger _changer;
        
        public event EventsHandler<int> VolumeUp;
        public event EventsHandler<int> VolumeDown;

        public FloatView()
        {
            InitializeComponent();
            this.PreviewMouseWheel += OnMouseWheel;
            _changer = this;
            _changer.VolumeDown += _changer_VolumeChanged;
            _changer.VolumeUp += _changer_VolumeChanged;
        }

        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!_model.EditMode && _model.AllowClick)
            {
                if (e.Delta < 0)
                    VolumeDown?.Invoke(this, new EventsArgs<int>(-1));
                else
                    VolumeUp?.Invoke(this, new EventsArgs<int>(1));
                e.Handled = true;
            }
        }

        private void _changer_VolumeChanged(object sender, Lazurite.Shared.EventsArgs<int> args)
        {
            //one big crutch --- value change need to be smooth, but mouse wheel sometimes can be fast
            if (SmoothChangeValueToSet == null)
                SmoothChangeValueToSet = _model.ScenarioValue;

            var iteration = (_model.Max - _model.Min) / 20;
            var delta = args.Value * iteration;
            var value = double.Parse(SmoothChangeValueToSet) + delta;
            if (value > _model.Max)
                value = _model.Max;
            if (value < _model.Min)
                value = _model.Min;
            SmoothChangeValueToSet = value.ToString();

            if (_smoothChangeValueThread == null)
            {
                _smoothChangeValueThread = new Thread(() => {
                    while (true)
                    {
                        var oldVal = SmoothChangeValueToSet;
                        Thread.Sleep(FloatView_SmoothChangeValueInterval);
                        if (oldVal == SmoothChangeValueToSet)
                        {
                            _model.ScenarioValue = SmoothChangeValueToSet;
                            _smoothChangeValueThread = null;
                            SmoothChangeValueToSet = null;
                            break;
                        }
                    }
                });
                _smoothChangeValueThread.IsBackground = true;
                _smoothChangeValueThread.Start();
            }
        }
        
        public FloatView(ScenarioBase scenario, UserVisualSettings visualSettings): this()
        {
            this.DataContext = _model = new ScenarioModel(scenario, visualSettings);
            //crutch
            _model.PropertyChanged += (o, e) =>
            {
                if (e.PropertyName == nameof(_model.ScenarioValue))
                    this.scaleView.Dispatcher.BeginInvoke(
                        new Action(() => this.scaleView.Value = double.Parse(_model.ScenarioValue)));
            };

            this.Loaded += (o, e) => this.scaleView.Value = double.Parse(_model.ScenarioValue);
        }

        private void itemView_Click(object sender, RoutedEventArgs e)
        {
            var floatSwitch = new FloatViewSwitch((ScenarioModel)this.DataContext);
            var dialog = new DialogView(floatSwitch);
            dialog.Show();
        }
    }
}
