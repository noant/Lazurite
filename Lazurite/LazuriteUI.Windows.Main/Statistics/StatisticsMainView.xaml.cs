using Lazurite.IOC;
using Lazurite.MainDomain;
using Lazurite.MainDomain.Statistics;
using LazuriteUI.Icons;
using LazuriteUI.Windows.Main.Statistics.Settings;
using LazuriteUI.Windows.Main.Statistics.Views;
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
        private static readonly IStatisticsManager StatisticsManager = Singleton.Resolve<IStatisticsManager>();
        private static readonly ScenariosRepositoryBase ScenariosRepository = Singleton.Resolve<ScenariosRepositoryBase>();
        private static readonly UsersRepositoryBase UsersRepository = Singleton.Resolve<UsersRepositoryBase>();
        private static readonly ScenarioActionSource SystemActionSource = new ScenarioActionSource(UsersRepository.SystemUser, ScenarioStartupSource.System, ScenarioAction.ViewValue);

        private IStatisticsView _currentView;

        public StatisticsMainView()
        {
            InitializeComponent();
        }
        
        private void btSettings_Click(object sender, RoutedEventArgs e)
        {
            StatisticsScenariosView.Show();
        }

        private void btTableView_Click(object sender, RoutedEventArgs e)
        {
            AppendView(new StatisticsTableView());
        }

        private void AppendView(IStatisticsView view)
        {
            _currentView = view;
            view.NeedItems = (filter) =>
            {
                if (!string.IsNullOrEmpty(filter.ScenarioId))
                {
                    var scenario = ScenariosRepository.Scenarios.First(x => x.Id == filter.ScenarioId);
                    var info = StatisticsManager.GetStatisticsInfoForScenario(scenario, SystemActionSource);
                    return StatisticsManager.GetItems(info, filter.Since, filter.To, SystemActionSource);
                }
                else
                {
                    return ScenariosRepository
                        .Scenarios
                        .Where(x => StatisticsManager.IsRegistered(x))
                        .Select(x => StatisticsManager.GetStatisticsInfoForScenario(x, SystemActionSource))
                        .SelectMany(x => StatisticsManager.GetItems(x, filter.Since, filter.To, SystemActionSource))
                        .ToArray();
                }
            };
            viewHostControl.Content = view;
        }
    }
}
