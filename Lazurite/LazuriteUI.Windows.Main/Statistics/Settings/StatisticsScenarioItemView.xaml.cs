using Lazurite.MainDomain;
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

namespace LazuriteUI.Windows.Main.Statistics.Settings
{
    /// <summary>
    /// Логика взаимодействия для StatisticsScenarioItemView.xaml
    /// </summary>
    public partial class StatisticsScenarioItemView : Grid
    {
        public StatisticsScenarioItemView(ScenarioBase scenario)
        {
            InitializeComponent();
            DataContext = new StatisticsScenarioItemModel(scenario);
        }
    }
}
