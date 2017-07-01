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
    public partial class DateTimeView : UserControl
    {
        public DateTimeView()
        {
            InitializeComponent();
        }

        public DateTimeView(ScenarioBase scenario, UserVisualSettings visualSettings): this()
        {
            this.DataContext = new ScenarioModel(scenario, visualSettings);
            itemView.Click += ItemView_Click;
        }

        private void ItemView_Click(object sender, RoutedEventArgs e)
        {
            var dateTimeSwitch = new DateTimeViewSwitch() {
                DateTime = DateTime.Parse(((ScenarioModel)this.DataContext).ScenarioValue)
            };
            var dialog = new DialogView(dateTimeSwitch);
            dateTimeSwitch.Apply += (o, args) => {
                dialog.Close();
                ((ScenarioModel)this.DataContext).ScenarioValue = dateTimeSwitch.DateTime.ToString();
            };
            dialog.Show();
        }
    }
}
