using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.IOC;
using Lazurite.MainDomain;
using Lazurite.MainDomain.Statistics;
using LazuriteUI.Windows.Controls;
using LazuriteUI.Windows.Main.Statistics.Views.GeolocationViewImplementation;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace LazuriteUI.Windows.Main.Statistics.Views
{
    /// <summary>
    /// Логика взаимодействия для GeolocationView.xaml
    /// </summary>
    public sealed partial class GeolocationView : Grid, IStatisticsView, IDisposable
    {
        private static readonly ScenariosRepositoryBase ScenariosRepository = Singleton.Resolve<ScenariosRepositoryBase>();
        private static readonly IStatisticsManager StatisticsManager = Singleton.Resolve<IStatisticsManager>();

        public GeolocationView()
        {
            InitializeComponent();

            DialogView.DialogOpened += DialogView_DialogOpened;
            DialogView.DialogClosed += DialogView_DialogClosed;

            locationsView.ScenarioSelectClicked += (o, e) => {
                SelectScenarioView.Show(
                    _scenarioIds,
                    (newScenarioIds) => {
                        _scenarioIds = newScenarioIds;
                        NeedItems?.Invoke(new StatisticsFilter()
                        {
                            ScenariosIds = newScenarioIds
                        });
                    });
            };

            Loaded += async (o, e) =>
            {
                var geolocationScenarios = ScenariosRepository
                    .Scenarios
                    .Where(x => x.ValueType is GeolocationValueType)
                    .ToArray();

                var registrationInfo = await StatisticsManager.GetRegistrationInfo(geolocationScenarios);

                _scenarioIds = registrationInfo.RegisteredIds;

                NeedItems?.Invoke(
                    new StatisticsFilter()
                    {
                        ScenariosIds = _scenarioIds
                    });
            };

            Unloaded += (o, e) => Dispose();
        }
        
        private void DialogView_DialogClosed(object sender, Lazurite.Shared.EventsArgs<object> args)
        {
            locationsView.Visibility = Visibility.Visible;
        }

        private void DialogView_DialogOpened(object sender, Lazurite.Shared.EventsArgs<object> args)
        {
            locationsView.Visibility = Visibility.Collapsed;
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

            var history = scenarios
                .Select(x => new LocationsView.GeolocationScenarioHistoryView() {
                    ScenarioInfo = x,
                    Datas = items
                        .Where(z => z.Target.ID == x.Id)
                        .Select(z => GeolocationData.FromString(z.Value))
                        .Where(z => 
                            !double.IsNaN(z.Latitude) && 
                            !double.IsNaN(z.Longtitude) &&
                            !double.IsInfinity(z.Latitude) &&
                            !double.IsInfinity(z.Longtitude))
                        .ToArray()
                }).ToArray();

            locationsView.RefreshWith(history);
        }

        public void Dispose()
        {
            DialogView.DialogOpened -= DialogView_DialogOpened;
            DialogView.DialogClosed -= DialogView_DialogClosed;
        }
    }
}
