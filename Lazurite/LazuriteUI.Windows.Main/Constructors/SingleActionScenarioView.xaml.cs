using HierarchicalData;
using Lazurite.IOC;
using Lazurite.MainDomain;
using Lazurite.Scenarios.ScenarioTypes;
using LazuriteUI.Windows.Main.Constructors.Decomposition;
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
            _actionView = new ActionView(Scenario.ActionHolder);
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

        public SingleActionScenario Scenario { get; private set; }
    }
}
