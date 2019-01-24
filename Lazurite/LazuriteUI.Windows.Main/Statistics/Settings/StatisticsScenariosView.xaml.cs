using Lazurite.IOC;
using Lazurite.MainDomain;
using Lazurite.MainDomain.Statistics;
using Lazurite.Scenarios.ScenarioTypes;
using LazuriteUI.Windows.Controls;
using System.Windows;
using System.Windows.Controls;

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
            InitializeInternal();
        }

        private async void InitializeInternal()
        {
            captionView.StartAnimateProgress();
            var registrationInfo = await StatisticsManager.GetRegistrationInfo(ScenariosRepository.Scenarios);
            foreach (var scenario in ScenariosRepository.Scenarios)
            {
                var item = new StatisticsScenarioItemView(scenario, registrationInfo.IsRegistered(scenario.Id) && (scenario.GetIsAvailable() || !(scenario is RemoteScenario)));
                spItems.Children.Add(item);
            }
            captionView.StopAnimateProgress();
            lblLoading.Visibility = Visibility.Collapsed;
        }

        public static void Show()
        {
            new DialogView(new StatisticsScenariosView()).Show();
        }
    }
}
