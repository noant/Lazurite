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

        public DiagramView()
        {
            InitializeComponent();

            Loaded += (o, e) => {
                var scenarios = ScenariosRepository.Scenarios.Where(x => x.ValueType is FloatValueType && StatisticsManager.IsRegistered(x)).ToArray();
                _infos = scenarios.Select(x => StatisticsManager.GetStatisticsInfoForScenario(x, SystemActionSource)).Take(5).ToArray();
                NeedItems?.Invoke(new StatisticsFilter()
                {
                    ScenariosIds = _infos.Select(x => x.ID).ToArray()
                });
            };
        }

        private StatisticsScenarioInfo[] _infos;

        public Action<StatisticsFilter> NeedItems { get; set; }

        public void RefreshItems(StatisticsItem[] items)
        {
            var diagrams = new List<IDiagramItem>();
            foreach (var info in _infos)
            {
                var scenarioName = info.Name;
                if (info.ValueTypeName == Lazurite.ActionsDomain.Utils.GetValueTypeClassName(typeof(FloatValueType)))
                {
                    var unit = (ScenariosRepository.Scenarios.FirstOrDefault(x => x.Id == info.ID)?.ValueType as FloatValueType).Unit?.Trim();
                    if (!string.IsNullOrEmpty(unit))
                        scenarioName += ", " + unit;
                }

                var diagram = new GraphicsDiagramItemView();
                var curItems = items.Where(x => x.Target.ID == info.ID).ToArray();
                diagram.SetPoints(scenarioName, curItems);
                diagrams.Add(diagram);
            }
            diagramsHost.SetItems(diagrams.ToArray());
        }
    }
}
