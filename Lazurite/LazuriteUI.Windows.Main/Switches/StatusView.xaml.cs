using Lazurite.MainDomain;
using LazuriteUI.Windows.Controls;
using System.Windows;
using System.Windows.Controls;

namespace LazuriteUI.Windows.Main.Switches
{
    /// <summary>
    /// Логика взаимодействия для ToggleView.xaml
    /// </summary>
    public partial class StatusView : UserControl
    {
        public StatusView()
        {
            InitializeComponent();
        }

        public StatusView(ScenarioBase scenario): this()
        {
            var model = new ScenarioModel(scenario);
            DataContext = model;
            Unloaded += (o, e) => model.Dispose();
            itemView.Click += ItemView_Click;
        }

        private void ItemView_Click(object sender, RoutedEventArgs e)
        {
            var statusSwitch = new StatusViewSwitch((ScenarioModel)DataContext);
            var dialog = new DialogView(statusSwitch);
            statusSwitch.StateChanged += (o, e2) => dialog.Close();
            dialog.Show();
        }
    }
}
