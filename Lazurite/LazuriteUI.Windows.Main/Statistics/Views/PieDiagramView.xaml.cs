using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.IOC;
using Lazurite.MainDomain;
using Lazurite.MainDomain.Statistics;
using LazuriteUI.Windows.Main.Statistics.Views.PieDiagramViewImplementation;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace LazuriteUI.Windows.Main.Statistics.Views
{
    /// <summary>
    /// Логика взаимодействия для PieDiagramView.xaml
    /// </summary>
    public partial class PieDiagramView : UserControl, IStatisticsView
    {
        private static readonly IStatisticsManager StatisticsManager = Singleton.Resolve<IStatisticsManager>();
        private static readonly ScenariosRepositoryBase ScenariosRepository = Singleton.Resolve<ScenariosRepositoryBase>();
        private static readonly UsersRepositoryBase UsersRepository = Singleton.Resolve<UsersRepositoryBase>();
        private static readonly ScenarioActionSource SystemActionSource = new ScenarioActionSource(UsersRepository.SystemUser, ScenarioStartupSource.System, ScenarioAction.ViewValue);

        public PieDiagramView()
        {
            InitializeComponent();

            Loaded += (o, e) => BeginSelectScenario();
        }

        private string _scenarioId;
        private StatisticsScenarioInfo _info;

        public Action<StatisticsFilter> NeedItems { get; set; }

        private void SelectScenarioClick(object sender, RoutedEventArgs e)
        {
            BeginSelectScenario();
        }

        private void BeginSelectScenario()
        {
            SelectScenarioView.Show(_scenarioId, (selectedScenarioId) => {
                _scenarioId = selectedScenarioId;
                Refresh();
            });
        }

        private void Refresh()
        {
            var scenario = ScenariosRepository.Scenarios.FirstOrDefault(x => x.Id == _scenarioId);
            var scenarioInfo = StatisticsManager.GetStatisticsInfoForScenario(scenario, SystemActionSource);
            NeedItems?.Invoke(new StatisticsFilter() {
                ScenariosIds = new[] { _scenarioId }
            });
        }

        public void RefreshItems(ScenarioStatistic[] scenarioStatistics, DateTime since, DateTime to)
        {
            var scenarioStatistic = scenarioStatistics.FirstOrDefault();
            _info = scenarioStatistic?.ScenarioInfo;
            var items = scenarioStatistic?.Statistic ?? new StatisticsItem[0];
            PieItemView prev = null;
            var views = items.Select(x =>
            {
                if (prev != null)
                    //calculating weights
                    prev.Weight = (x.DateTime - prev.DateTime).Seconds;

                var weight = 0;
                if (x == items.Last())
                    weight = (DateTime.Now - x.DateTime).Seconds;

                return prev = new PieItemView()
                {
                    DayOfWeek = GetDayOfWeek(x.DateTime.DayOfWeek),
                    UserName = x.SourceName,
                    Value = GetValue(x),
                    DateTime = x.DateTime,
                    Weight = weight
                };
            }).ToArray();

            var viewsByVal = views.Select(x => x.Value).Distinct().Select(x => new StatisticsCategoryView()
            {
                Category = x,
                Weight = views.Where(z => z.Value == x).Sum(z => z.Weight)
            }).ToArray();

            var viewsByUser = views.Select(x => x.UserName).Distinct().Select(x => new StatisticsCategoryView()
            {
                Category = x,
                Weight = views.Count(z => z.UserName == x)
            }).ToArray();

            var viewsByWeek = views.Select(x => x.DayOfWeek).Distinct().Select(x => new StatisticsCategoryView()
            {
                Category = x,
                Weight = views.Count(z => z.DayOfWeek == x)
            }).ToArray();

            chartByVal.SetItems(viewsByVal);
            chartByUser.SetItems(viewsByUser);
            chartByWeek.SetItems(viewsByWeek);

            lblEmpty.Visibility = items.Any() ? Visibility.Collapsed : Visibility.Visible;
            lblCaption.Content = _info?.Name;
        }

        private string GetValue(StatisticsItem item)
        {
            if (_info.ValueTypeName == Lazurite.ActionsDomain.Utils.GetValueTypeClassName(typeof(ToggleValueType)))
                return item.Value == ToggleValueType.ValueOFF ? "Выкл." : "Вкл.";
            else return item.Value;
        }

        private string GetDayOfWeek(DayOfWeek day)
        {
            switch (day)
            {
                case DayOfWeek.Friday:
                    return "Пятница";
                case DayOfWeek.Monday:
                    return "Понедельник";
                case DayOfWeek.Saturday:
                    return "Суббота";
                case DayOfWeek.Sunday:
                    return "Воскресенье";
                case DayOfWeek.Tuesday:
                    return "Вторник";
                case DayOfWeek.Thursday:
                    return "Четверг";
                case DayOfWeek.Wednesday:
                    return "Среда";
            }
            throw new InvalidOperationException();
        }

        private class PieItemView
        {
            public string Value { get; set; }
            public string UserName { get; set; }
            public string DayOfWeek { get; set; }
            public DateTime DateTime { get; set; }
            public int Weight { get; set; }
        }
    }
}
