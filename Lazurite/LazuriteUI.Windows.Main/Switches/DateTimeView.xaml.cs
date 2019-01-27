using Lazurite.MainDomain;
using LazuriteUI.Windows.Controls;
using System;
using System.Windows;
using System.Windows.Controls;

namespace LazuriteUI.Windows.Main.Switches
{
    /// <summary>
    /// Логика взаимодействия для ToggleView.xaml
    /// </summary>
    public partial class DateTimeView : UserControl
    {
        public DateTimeView()
        {
            InitializeComponent();
        }

        public DateTimeView(ScenarioBase scenario): this()
        {
            var model = new ScenarioModel(scenario);
            DataContext = model;
            Unloaded += (o, e) => model.Dispose();
            itemView.Click += ItemView_Click;
        }

        private void ItemView_Click(object sender, RoutedEventArgs e)
        {
            var dateTime = DateTime.Now;
            DateTime.TryParse(((ScenarioModel)DataContext).ScenarioValue, out dateTime);
            var dateTimeSwitch = new DateTimeViewSwitch() {
                DateTime = dateTime
            };
            var dialog = new DialogView(dateTimeSwitch);
            dateTimeSwitch.Apply += (o, args) => {
                dialog.Close();
                ((ScenarioModel)DataContext).ScenarioValue = dateTimeSwitch.DateTime.ToString();
            };
            dialog.Show();
        }
    }
}
