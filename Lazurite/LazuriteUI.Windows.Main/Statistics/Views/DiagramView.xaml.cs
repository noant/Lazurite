using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.Data;
using Lazurite.IOC;
using Lazurite.MainDomain;
using Lazurite.MainDomain.Statistics;
using Lazurite.Scenarios.ScenarioTypes;
using LazuriteUI.Windows.Main.Statistics.Views.DiagramViewImplementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace LazuriteUI.Windows.Main.Statistics.Views
{
    /// <summary>
    /// Логика взаимодействия для DaigramView.xaml
    /// </summary>
    public partial class DiagramView : UserControl, IStatisticsView
    {
        private static readonly IStatisticsManager StatisticsManager = Singleton.Resolve<IStatisticsManager>();
        private static readonly ScenariosRepositoryBase ScenariosRepository = Singleton.Resolve<ScenariosRepositoryBase>();
        private static readonly UsersRepositoryBase UsersRepository = Singleton.Resolve<UsersRepositoryBase>();
        private static readonly ScenarioActionSource SystemActionSource = new ScenarioActionSource(UsersRepository.SystemUser, ScenarioStartupSource.System, ScenarioAction.ViewValue);
        private static readonly DataManagerBase DataManager = Singleton.Resolve<DataManagerBase>();

        public DiagramView()
        {
            InitializeComponent();

            diagramsHost.ScenariosSelectPressed += (o, e) => BeginSelectScenarios();

            Loaded += (o, e) =>
            {
                if (LoadData())
                    InitiateReload();
                else
                    BeginSelectScenarios();
            };
        }

        private string[] _diagramsScenariosViews;
        private StatisticsScenarioInfo[] _infos;
        public Action<StatisticsFilter> NeedItems { get; set; }
        
        private bool LoadData()
        {
            if (!DataManager.Has(nameof(_diagramsScenariosViews)))
                return false;
            _diagramsScenariosViews = DataManager.Get<string[]>(nameof(_diagramsScenariosViews));
            return _diagramsScenariosViews.Any();
        }

        private void SaveData()
        {
            DataManager.Set(nameof(_diagramsScenariosViews), _diagramsScenariosViews);
        }

        private void BeginSelectScenarios()
        {
            SelectScenarioView.Show(_diagramsScenariosViews, (newScenarios) => {
                _diagramsScenariosViews = newScenarios;
                SaveData();
                InitiateReload();
            });
        }

        private async void InitiateReload()
        {
            var selectedScenarios = 
                ScenariosRepository
                .Scenarios
                .Where(x => _diagramsScenariosViews.Contains(x.Id))
                .ToArray();

            var registrationInfo = await StatisticsManager.GetRegistrationInfo(selectedScenarios);

            var scenarios = 
                selectedScenarios
                .Where(x => registrationInfo.IsRegistered(x.Id) && (x.GetIsAvailable() || !(x is RemoteScenario)))
                .ToArray();

            _infos = 
                await Task.WhenAll(
                    scenarios
                    .Select(x => StatisticsManager.GetStatisticsInfoForScenario(x, SystemActionSource)));

            NeedItems?.Invoke(new StatisticsFilter()
            {
                ScenariosIds = _infos.Select(x => x.ID).ToArray()
            });
        }

        public void RefreshItems(ScenarioStatistic[] scenarioStatistics, DateTime since, DateTime to)
        {
            void refresh ()
            {
                diagramsHost.MaxDate = to;
                diagramsHost.MinDate = since;
                var diagrams = new List<IDiagramItem>();

                if (!scenarioStatistics.Any() || scenarioStatistics.Sum(x => x.Statistic.Length) == 0)
                    lblDataEmpty.Visibility = Visibility.Visible;
                else
                {
                    lblDataEmpty.Visibility = Visibility.Collapsed;
                    diagramsHost.Visibility = Visibility.Visible;
                    foreach (var info in _infos)
                    {
                        var scenarioName = info.Name;
                        IDiagramItem diagram = null;
                        if (info.ValueTypeName == Lazurite.ActionsDomain.Utils.GetValueTypeClassName(typeof(FloatValueType)))
                        {
                            var unit = (ScenariosRepository.Scenarios.FirstOrDefault(x => x.Id == info.ID)?.ValueType as FloatValueType).Unit?.Trim();
                            if (!string.IsNullOrEmpty(unit))
                                scenarioName += ", " + unit;
                            diagram = new GraphicsDiagramItemView();
                        }
                        else if (info.ValueTypeName == Lazurite.ActionsDomain.Utils.GetValueTypeClassName(typeof(StateValueType)))
                            diagram = new StatesDiagramItemView();
                        else if (info.ValueTypeName == Lazurite.ActionsDomain.Utils.GetValueTypeClassName(typeof(ToggleValueType)))
                            diagram = new ToggleDiagramItemView();
                        else
                            diagram = new InfoDiagramItemView();
                        var curItems = scenarioStatistics.FirstOrDefault(x => x.ScenarioInfo.ID == info.ID).Statistic;
                        diagram.Points = scenarioStatistics.First(x => x.ScenarioInfo.ID == info.ID);
                        diagrams.Add(diagram);
                    }

                    var ordered = diagrams
                        .OrderByDescending(x => x is ToggleDiagramItemView)
                        .OrderByDescending(x => x is StatesDiagramItemView)
                        .OrderByDescending(x => x is GraphicsDiagramItemView)
                        .ToArray();

                    diagramsHost.SetItems(ordered);
                }

            }

            if (IsLoaded)
                refresh();
            else Loaded += (o, e) => refresh();
        }
    }
}