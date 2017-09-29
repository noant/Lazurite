using Lazurite.ActionsDomain.ValueTypes;
using RunProcessPlugin;
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

namespace test
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        RunProcessAction _action;
        public MainWindow()
        {
            InitializeComponent();
            var action = new RunProcessAction();
            action.UserInitializeWith(new ToggleValueType(), false);

            btSettings.Click += (o,e) => action.UserInitializeWith(new ToggleValueType(), false);

            action.ValueChanged += (o, e) => {
                this.Dispatcher.BeginInvoke((Action)(() => {
                    if (e == ToggleValueType.ValueOFF)
                        btStart.Content = "START";
                    else btStart.Content = "STOP";
                }));
            };

            btStart.Click += (o, e) => {
                if (btStart.Content.ToString() == "START")
                {
                    action.SetValue(null, ToggleValueType.ValueON);
                    btStart.Content = "STOP";
                }
                else
                {
                    action.SetValue(null, ToggleValueType.ValueOFF);
                    btStart.Content = "START";
                }
            };
        }
    }
}
