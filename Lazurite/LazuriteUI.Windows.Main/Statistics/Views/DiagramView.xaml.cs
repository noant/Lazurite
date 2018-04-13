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
using Lazurite.Data;
using Lazurite.IOC;
using Lazurite.MainDomain;
using Lazurite.MainDomain.Statistics;
using LazuriteUI.Windows.Main.Statistics.Views.DiagramViewImplementation;

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
        private static readonly SaviorBase Savior = Singleton.Resolve<SaviorBase>();

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
            if (!Savior.Has(nameof(_diagramsScenariosViews)))
                return false;
            _diagramsScenariosViews = Savior.Get<string[]>(nameof(_diagramsScenariosViews));
            return _diagramsScenariosViews.Any();
        }

        private void SaveData()
        {
            Savior.Set(nameof(_diagramsScenariosViews), _diagramsScenariosViews);
        }

        private void BeginSelectScenarios()
        {
            SelectScenarioView.Show(_diagramsScenariosViews, (newScenarios) => {
                _diagramsScenariosViews = newScenarios;
                SaveData();
                InitiateReload();
            });
        }

        private void InitiateReload()
        {
            var scenarios = ScenariosRepository.Scenarios
                    .Where(x => _diagramsScenariosViews.Contains(x.Id) && StatisticsManager.IsRegistered(x))
                    .ToArray();
            _infos = scenarios.Select(x => StatisticsManager.GetStatisticsInfoForScenario(x, SystemActionSource)).ToArray();
            NeedItems?.Invoke(new StatisticsFilter()
            {
                ScenariosIds = _infos.Select(x => x.ID).ToArray()
            });
        }

        public void RefreshItems(StatisticsItem[] items, DateTime since, DateTime to)
        {
            diagramsHost.MaxDate = to;
            diagramsHost.MinDate = since;
            var diagrams = new List<IDiagramItem>();
            if (items.Any())
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
                    var curItems = items.Where(x => x.Target.ID == info.ID).ToArray();
                    diagram.SetPoints(scenarioName, curItems);
                    diagrams.Add(diagram);
                }
                var ordered = diagrams
                    .OrderByDescending(x => x is ToggleDiagramItemView)
                    .OrderByDescending(x => x is StatesDiagramItemView)
                    .OrderByDescending(x => x is GraphicsDiagramItemView)
                    .ToArray();
                diagramsHost.SetItems(ordered);
            }

            if (!items.Any() || !diagrams.Any())
                lblDataEmpty.Visibility = Visibility.Visible;
        }
    }
}