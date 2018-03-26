using LazuriteUI.Icons;
using LazuriteUI.Windows.Main.Statistics.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace LazuriteUI.Windows.Main.Statistics
{
    /// <summary>
    /// Логика взаимодействия для StatisticsMainView.xaml
    /// </summary>
    [LazuriteIcon(Icon.GraphLine)]
    [DisplayName("Статистика")]
    public partial class StatisticsMainView : Grid
    {
        public StatisticsMainView()
        {
            InitializeComponent();
        }
        
        private void btSettings_Click(object sender, RoutedEventArgs e)
        {
            StatisticsScenariosView.Show();
        }
    }
}
