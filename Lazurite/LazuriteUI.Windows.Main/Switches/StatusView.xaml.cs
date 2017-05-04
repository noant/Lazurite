using Lazurite.ActionsDomain;
using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.MainDomain;
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
            this.DataContext = new ScenarioModel(scenario);
            itemView.Click += ItemView_Click;
        }

        private void ItemView_Click(object sender, RoutedEventArgs e)
        {
            var statusSwitch = new StatusViewSwitch((ScenarioModel)this.DataContext);
            var dialog = new DialogView(statusSwitch);
            statusSwitch.StateChanged += (o, e2) => dialog.Close();
            dialog.Show(Window.GetWindow(this).Content as Grid);
        }
    }
}
