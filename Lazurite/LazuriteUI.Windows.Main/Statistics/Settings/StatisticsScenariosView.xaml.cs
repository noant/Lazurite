using Lazurite.IOC;
using Lazurite.MainDomain;
using Lazurite.MainDomain.Statistics;
using Lazurite.Scenarios.ScenarioTypes;
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

namespace LazuriteUI.Windows.Main.Statistics.Settings
{
    /// <summary>
    /// Логика взаимодействия для StatisticsScenariosView.xaml
    /// </summary>
    public partial class StatisticsScenariosView : Grid
    {
        private static readonly ScenariosRepositoryBase ScenariosRepository = Singleton.Resolve<ScenariosRepositoryBase>();
        private static readonly IStatisticsManager StatisticsManager = Singleton.Resolve<IStatisticsManager>();

        public StatisticsScenariosView()
        {
            InitializeComponent();
            var registrationInfo = StatisticsManager.GetRegistrationInfo(ScenariosRepository.Scenarios);
            foreach (var scenario in ScenariosRepository.Scenarios)
            {
                var item = new StatisticsScenarioItemView(scenario, registrationInfo.IsRegistered(scenario.Id) && (scenario.GetIsAvailable() || !(scenario is RemoteScenario)));
                spItems.Children.Add(item);
            }
        }

        public static void Show()
        {
            new DialogView(new StatisticsScenariosView()).Show();
        }
    }
}
