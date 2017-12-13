using Lazurite.MainDomain;
using Lazurite.Scenarios.ScenarioTypes;
using LazuriteUI.Windows.Main.Constructors.Decomposition;
using System;
using System.Windows.Controls;

namespace LazuriteUI.Windows.Main.Constructors
{
    /// <summary>
    /// Логика взаимодействия для SingleActionScenarioView.xaml
    /// </summary>
    public partial class SingleActionScenarioView : UserControl, IScenarioConstructorView
    {
        private ActionView _actionView;

        public SingleActionScenarioView(SingleActionScenario scenario)
        {
            InitializeComponent();
            Scenario = scenario;
            Initialize();
        }
        
        private void Initialize()
        {
            _actionView = new ActionView();
            _actionView.Refresh(Scenario.ActionHolder, Scenario);
            _actionView.Modified += (element) => Modified?.Invoke();
            gridContent.Children.Add(_actionView);
        }

        private void ReInitialize()
        {
            gridContent.Children.Clear();
            Initialize();
        }
        
        public void Revert(ScenarioBase scenario)
        {
            Scenario = (SingleActionScenario)scenario;
            ReInitialize();
        }
        
        public event Action Modified;
        public event Action Failed;
        public event Action Succeed;

        public SingleActionScenario Scenario { get; private set; }
    }
}
