using Lazurite.MainDomain;
using System.Windows.Controls;

namespace LazuriteUI.Windows.Main.Statistics.Settings
{
    /// <summary>
    /// Логика взаимодействия для StatisticsScenarioItemView.xaml
    /// </summary>
    public partial class StatisticsScenarioItemView : Grid
    {
        public StatisticsScenarioItemView(ScenarioBase scenario, bool registered)
        {
            InitializeComponent();
            DataContext = new StatisticsScenarioItemModel(scenario, registered);
        }
    }
}
