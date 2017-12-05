using Lazurite.ActionsDomain.ValueTypes;
using RunProcessPlugin;
using System;
using System.Windows;

namespace test
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
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
