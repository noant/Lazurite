using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.IOC;
using Lazurite.MainDomain;
using Lazurite.MainDomain.Statistics;
using LazuriteUI.Windows.Main.Statistics.Views.PieDiagramViewImplementation;
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

        public void RefreshItems(StatisticsItem[] items, DateTime since, DateTime to)
        {
            var views = items.Select(x => new PieItemView()
            {
                DayOfWeek = GetDayOfWeek(x.DateTime.DayOfWeek),
                UserName = x.Source.Name,
                Value = GetValue(x)
            }).ToArray();

            var viewsByVal = views.Select(x => x.Value).Distinct().Select(x => new StatisticsCategoryView()
            {
                Category = x,
                Count = views.Count(z => z.Value == x)
            }).ToArray();

            var viewsByUser = views.Select(x => x.UserName).Distinct().Select(x => new StatisticsCategoryView()
            {
                Category = x,
                Count = views.Count(z => z.UserName == x)
            }).ToArray();

            var viewsByWeek = views.Select(x => x.DayOfWeek).Distinct().Select(x => new StatisticsCategoryView()
            {
                Category = x,
                Count = views.Count(z => z.DayOfWeek == x)
            }).ToArray();

            chartByVal.SetItems(viewsByVal);
            chartByUser.SetItems(viewsByUser);
            chartByWeek.SetItems(viewsByWeek);

            lblEmpty.Visibility = items.Any() ? Visibility.Collapsed : Visibility.Visible;
        }

        private string GetValue(StatisticsItem item)
        {
            if (item.Target.ValueTypeName == Lazurite.ActionsDomain.Utils.GetValueTypeClassName(typeof(ToggleValueType)))
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
        }
    }
}
