using Lazurite.IOC;
using Lazurite.MainDomain;
using Lazurite.Scenarios.ScenarioTypes;
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
    /// Логика взаимодействия для ScenariosResolverView.xaml
    /// </summary>
    public partial class ScenariosConstructorsResolverView : UserControl
    {
        private ScenariosRepositoryBase _repository = Singleton.Resolve<ScenariosRepositoryBase>(); 
        private IScenarioConstructorView _constructorView;
        private ScenarioBase _originalSenario;
        private ScenarioBase _clonedScenario;

        public event Action Modified;

        public ScenariosConstructorsResolverView()
        {
            InitializeComponent();
        }

        public void SetScenario(ScenarioBase scenario)
        {
            _originalSenario = scenario;
            _clonedScenario = (ScenarioBase)Lazurite.Windows.Utils.Utils.CloneObject(_originalSenario);
            if (scenario is SingleActionScenario)
                this.Content = _constructorView = new SingleActionScenarioView((SingleActionScenario)_clonedScenario);

            _constructorView.Modified += () => Modified?.Invoke();
        }

        public ScenarioBase Apply()
        {
            _originalSenario.TryCancelAll();
            _repository.SaveScenario(_clonedScenario);
            _originalSenario = _clonedScenario;
            _originalSenario.Initialize(_repository);
            _clonedScenario = (ScenarioBase)Lazurite.Windows.Utils.Utils.CloneObject(_originalSenario);
            return _originalSenario;
        }

        public void Revert()
        {
            _clonedScenario = (ScenarioBase)Lazurite.Windows.Utils.Utils.CloneObject(_originalSenario);
            _constructorView.Revert(_clonedScenario);
        }
    }
}
