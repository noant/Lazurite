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
using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.IOC;
using Lazurite.MainDomain;
using Lazurite.MainDomain.Statistics;
using LazuriteUI.Windows.Main.Statistics.Views.GeolocationViewImplementation;

namespace LazuriteUI.Windows.Main.Statistics.Views
{
    /// <summary>
    /// Логика взаимодействия для GeolocationView.xaml
    /// </summary>
    public partial class GeolocationView : UserControl, IStatisticsView
    {
        private static readonly ScenariosRepositoryBase ScenariosRepository = Singleton.Resolve<ScenariosRepositoryBase>();
        private static readonly IStatisticsManager StatisticsManager = Singleton.Resolve<IStatisticsManager>();

        public GeolocationView()
        {
            InitializeComponent();

            _scenarioIds = 
                ScenariosRepository.Scenarios
                .Where(x => x.ValueType is GeolocationValueType && StatisticsManager.IsRegistered(x))
                .Select(x => x.Id)
                .ToArray();

            NeedItems?.Invoke(
                new StatisticsFilter() {
                    ScenariosIds = _scenarioIds
                });

            locationsView.ScenarioSelectClicked += (o, e) => {
                locationsView.Visibility = Visibility.Collapsed;
                SelectScenarioView.Show(
                    _scenarioIds, 
                    (newScenarioIds) => {
                        _scenarioIds = newScenarioIds;
                        NeedItems?.Invoke(new StatisticsFilter()
                        {
                            ScenariosIds = newScenarioIds
                        });
                    }, 
                    ()=> {
                        locationsView.Visibility = Visibility.Visible;
                    });
            };
        }

        private string[] _scenarioIds;

        public Action<StatisticsFilter> NeedItems { get; set; }

        public void RefreshItems(StatisticsItem[] items, DateTime since, DateTime to)
        {
            var scenarios = items.Select(x => 
                new LocationsView.ScenarioInfo() {
                    Id = x.Target.ID,
                    Name = x.Target.Name
                })
            .Distinct()
            .ToArray();

            var history = scenarios.Select(x => new LocationsView.GeolocationScenarioHistoryView() {
                ScenarioInfo = x,
                Datas = items
                    .Where(z => z.Target.ID == x.Id)
                    .Select(z => GeolocationData.FromString(z.Value))
                    .ToArray()
            }).ToArray();

            locationsView.RefreshWith(history);
        }
    }
}
