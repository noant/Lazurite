using Lazurite.MainDomain;
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
        private volatile string _tempValue;
        private Task _syncTask;
        private CancellationTokenSource _tokenSource = new CancellationTokenSource(); 

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
            this.DataContext = model;
            this._tempValue = model.ScenarioValue;

            this.slider.Value = double.Parse(_tempValue ?? "0");
            this.slider.ValueChanged += (o, e) =>
            {
                _tempValue = slider.Value.ToString();
            };

            _syncTask = Task.Factory.StartNew(() => {
                while (!_tokenSource.IsCancellationRequested)
                {
                    if (_tempValue != model.ScenarioValue)
                        model.ScenarioValue = _tempValue;
                    Thread.Sleep(500);
                }
            });
        }
    }
}
