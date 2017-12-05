using Lazurite.ActionsDomain;
using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.IOC;
using Lazurite.MainDomain;
using Lazurite.Utils;
using LazuriteUI.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Lazurite.Shared;

namespace LazuriteUI.Windows.Main.Switches
{
    /// <summary>
    /// Логика взаимодействия для ToggleView.xaml
    /// </summary>
    public partial class FloatView : UserControl, IHardwareVolumeChanger
    {
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
            if (e.Delta < 0)
                VolumeDown?.Invoke(this, new EventsArgs<int>(-1));
            else
                VolumeUp?.Invoke(this, new EventsArgs<int>(1));
        }

        private void _changer_VolumeChanged(object sender, Lazurite.Shared.EventsArgs<int> args)
        {
            var iteration = (_model.Max - _model.Min) / 20;
            var delta = args.Value * iteration;
            var value = double.Parse(_model.ScenarioValue) + delta;
            if (value > _model.Max)
                value = _model.Max;
            if (value < _model.Min)
                value = _model.Min;
            _model.ScenarioValue = value.ToString();
        }
        
        public FloatView(ScenarioBase scenario, UserVisualSettings visualSettings): this()
        {
            _model = new ScenarioModel(scenario, visualSettings);
            this.DataContext = _model;
        }

        private void itemView_Click(object sender, RoutedEventArgs e)
        {
            var floatSwitch = new FloatViewSwitch((ScenarioModel)this.DataContext);
            var dialog = new DialogView(floatSwitch);
            dialog.Show();
        }
    }
}
