using Lazurite.MainDomain;
using Lazurite.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace LazuriteUI.Windows.Main.Switches
{
    /// <summary>
    /// Логика взаимодействия для FloatView.xaml
    /// </summary>
    public partial class FloatViewSwitch : UserControl, IDisposable
    {
        private static readonly int FloatView_ValueUpdateInterval = GlobalSettings.Get(nameof(FloatView_ValueUpdateInterval), 300);

        private volatile string _tempValue;
        private CancellationTokenSource _tokenSource = new CancellationTokenSource();
        private ScenarioModel _model;
        private double _iteration; 

        public FloatViewSwitch()
        {
            InitializeComponent();
        }

        public void Dispose()
        {
            _tokenSource.Cancel();
        }

        public FloatViewSwitch(ScenarioModel model): this()
        {
            this.DataContext = _model = model;
            this._tempValue = model.ScenarioValue;
            this._iteration = (model.Max - model.Min) / 20;
            this.slider.Value = double.Parse(_tempValue ?? "0");
            this.slider.ValueChanged += (o, e) =>
            {
                _tempValue = slider.Value.ToString();
            };

            TaskUtils.Start(() => {
                while (!_tokenSource.IsCancellationRequested)
                {
                    if (_tempValue != model.ScenarioValue)
                        model.ScenarioValue = _tempValue;
                    Thread.Sleep(FloatView_ValueUpdateInterval);
                }
            });
        }

        private void slider_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta < 0)
                this.slider.Value -= _iteration;
            else this.slider.Value += _iteration;
        }
    }
}
