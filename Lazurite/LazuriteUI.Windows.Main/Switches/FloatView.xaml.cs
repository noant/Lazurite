using Lazurite.IOC;
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
        private static readonly ISystemUtils SystemUtils = Singleton.Resolve<ISystemUtils>();
        private static readonly int FloatView_SmoothChangeValueInterval = GlobalSettings.Get(300);

        private CancellationTokenSource _smoothTimerCancellationToken;

        private double SmoothChangeValueToSet
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
            PreviewMouseWheel += OnMouseWheel;
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
            
            var iteration = (_model.Max - _model.Min) / 20;
            var delta = args.Value * iteration;
            var value = SmoothChangeValueToSet + delta;
            if (value > _model.Max)
                value = _model.Max;
            if (value < _model.Min)
                value = _model.Min;
            SmoothChangeValueToSet = value;

            if (_smoothTimerCancellationToken == null)
            {
                var oldVal = SmoothChangeValueToSet;
                _smoothTimerCancellationToken = SystemUtils.StartTimer(
                    (token) => {
                        if (oldVal == SmoothChangeValueToSet)
                        {
                            _model.ScenarioValue = SmoothChangeValueToSet.ToString();
                            _smoothTimerCancellationToken.Cancel();
                            _smoothTimerCancellationToken = null;
                            return;
                        }
                        oldVal = SmoothChangeValueToSet;
                    }, 
                    () => FloatView_SmoothChangeValueInterval);
            }
        }
        
        public FloatView(ScenarioBase scenario, UserVisualSettings visualSettings): this()
        {
            DataContext = _model = new ScenarioModel(scenario, visualSettings);
            //crutch
            _model.PropertyChanged += (o, e) =>
            {
                if (e.PropertyName == nameof(_model.ScenarioValue))
                    scaleView.Dispatcher.BeginInvoke(
                        new Action(() => scaleView.Value = _model.ScenarioValueDouble));
            };

            Loaded += (o, e) => scaleView.Value = _model.ScenarioValueDouble;
        }

        private void itemView_Click(object sender, RoutedEventArgs e)
        {
            var floatSwitch = new FloatViewSwitch((ScenarioModel)DataContext);
            var dialog = new DialogView(floatSwitch);
            dialog.Show();
        }
    }
}
