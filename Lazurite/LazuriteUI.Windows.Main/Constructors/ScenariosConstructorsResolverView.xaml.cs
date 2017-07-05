using Lazurite.IOC;
using Lazurite.MainDomain;
using Lazurite.Scenarios.ScenarioTypes;
using LazuriteUI.Windows.Controls;
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

        public event Action Applied;
        public event Action Modified;
        public bool IsModified { get; private set; }

        public ScenariosConstructorsResolverView()
        {
            InitializeComponent();
            this.buttonsView.ApplyClicked += () => Apply();
            this.buttonsView.ResetClicked += () => Revert();
        }

        public void SetScenario(ScenarioBase scenario)
        {
            if (scenario != null)
            {
                _originalSenario = scenario;
                _clonedScenario = (ScenarioBase)Lazurite.Windows.Utils.Utils.CloneObject(_originalSenario);
                _clonedScenario.Initialize(_repository);
                if (scenario is SingleActionScenario)
                    this.contentPresenter.Content = _constructorView = new SingleActionScenarioView((SingleActionScenario)_clonedScenario);
                else if (scenario is RemoteScenario)
                    this.contentPresenter.Content = _constructorView = new RemoteScenarioView((RemoteScenario)_clonedScenario);
                else if (scenario is CompositeScenario)
                    this.contentPresenter.Content = _constructorView = new CompositeScenarioView((CompositeScenario)_clonedScenario);
                buttonsView.SetScenario(_clonedScenario);
                buttonsView.Modified += () => IsModified = true;
                _constructorView.Modified += () => Modified?.Invoke();
                _constructorView.Modified += () => buttonsView.ScenarioModified();
                _constructorView.Modified += () => IsModified = true;
                _constructorView.Failed += () => buttonsView.Failed();
                _constructorView.Succeed += () => buttonsView.Success();
                EmptyScenarioModeOff();
            }
            else
            {
                EmptyScenarioModeOn();
            }
            IsModified = false;
        }

        private void EmptyScenarioModeOn()
        {
            tbScenarioEmpty.Visibility = Visibility.Visible;
            buttonsViewHolder.Visibility = Visibility.Collapsed;
            this.contentPresenter.Content = null;
        }

        private void EmptyScenarioModeOff()
        {
            tbScenarioEmpty.Visibility = Visibility.Collapsed;
            buttonsViewHolder.Visibility = Visibility.Visible;
        }

        public ScenarioBase GetScenario()
        {
            return _originalSenario;
        }

        public void Apply(Action callback = null)
        {
            if (_originalSenario.ValueType != null && !_originalSenario.ValueType.IsCompatibleWith(_clonedScenario.ValueType))
            {
                MessageView.ShowYesNo(
                    "Сценарий был изменен так, что тип изначального сценария не совместим с типом текущего сценария.\r\n" +
                    "Если на этот сценарий ссылаются удаленные или комплексные сценарии, то они могут выполняться с ошибкой.\r\n" +
                    "Сохранить сценарий?",
                    "Сохранение сценария",
                    Icons.Icon.Warning,
                    (result) =>
                    {
                        ApplyInternal();
                        callback?.Invoke();
                    });
            }
            else
            {
                ApplyInternal();
                callback?.Invoke();
            }
        }

        private void ApplyInternal()
        {
            _originalSenario.TryCancelAll();
            _repository.SaveScenario(_clonedScenario);
            _clonedScenario.Initialize(_repository);
            _clonedScenario.AfterInitilize();
            SetScenario(_clonedScenario);
            Applied?.Invoke();
            IsModified = false;
        }

        public void Revert()
        {
            _clonedScenario = (ScenarioBase)Lazurite.Windows.Utils.Utils.CloneObject(_originalSenario);
            _clonedScenario.Initialize(_repository);
            buttonsView.Revert(_clonedScenario);
            _constructorView.Revert(_clonedScenario);
            IsModified = false;
        }
    }
}