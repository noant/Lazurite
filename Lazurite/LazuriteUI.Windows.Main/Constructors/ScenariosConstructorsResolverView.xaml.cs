using Lazurite.IOC;
using Lazurite.Logging;
using Lazurite.MainDomain;
using Lazurite.Scenarios.ScenarioTypes;
using LazuriteUI.Windows.Controls;
using System;
using System.Windows;
using System.Windows.Controls;

namespace LazuriteUI.Windows.Main.Constructors
{
    /// <summary>
    /// Логика взаимодействия для ScenariosResolverView.xaml
    /// </summary>
    public partial class ScenariosConstructorsResolverView : UserControl
    {
        private ILogger _log = Singleton.Resolve<ILogger>();
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
            buttonsView.ApplyClicked += () => Apply();
            buttonsView.ResetClicked += () => Revert();
            buttonsView.Modified += () => IsModified = true;
        }

        public void SetScenario(ScenarioBase scenario, Action callback = null)
        {
            StuckUILoadingWindow.Show(
                "Компоновка окна...",
                () =>
                {
                    if (scenario != null)
                    {
                        try
                        {
                            _originalSenario = scenario;
                            _clonedScenario = (ScenarioBase)Lazurite.Windows.Utils.Utils.CloneObject(_originalSenario);
                            _clonedScenario.Initialize();
                            if (scenario is SingleActionScenario)
                                contentPresenter.Content = _constructorView = new SingleActionScenarioView((SingleActionScenario)_clonedScenario);
                            else if (scenario is RemoteScenario)
                                contentPresenter.Content = _constructorView = new RemoteScenarioView((RemoteScenario)_clonedScenario);
                            else if (scenario is CompositeScenario)
                                contentPresenter.Content = _constructorView = new CompositeScenarioView((CompositeScenario)_clonedScenario);
                            buttonsView.SetScenario(_clonedScenario);
                            IsModified = false;
                            _constructorView.Modified += () => Modified?.Invoke();
                            _constructorView.Modified += () => buttonsView.ScenarioModified();
                            _constructorView.Modified += () => IsModified = true;
                            _constructorView.Failed += () => buttonsView.Failed();
                            _constructorView.Succeed += () => buttonsView.Success();
                            EmptyScenarioModeOff();
                        }
                        catch (Exception e)
                        {
                            _log.ErrorFormat(e, "Ошибка инициализации сценария {0}", scenario.Name);
                        }
                    }
                    else
                    {
                        EmptyScenarioModeOn();
                    }
                    callback?.Invoke();
                }
            );
        }

        private void EmptyScenarioModeOn()
        {
            tbScenarioEmpty.Visibility = Visibility.Visible;
            buttonsViewHolder.Visibility = Visibility.Collapsed;
            contentPresenter.Content = null;
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

        public void Apply(Action callback = null, bool reset = true)
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
                        if (result)
                            ApplyInternal(reset);
                        callback?.Invoke();
                    });
            }
            else
            {
                ApplyInternal(reset);
                callback?.Invoke();
            }
        }

        private void ApplyInternal(bool reset = true)
        {
            try
            {
                _repository.SaveScenario(_clonedScenario);
                _clonedScenario.Initialize();
                _clonedScenario.AfterInitilize();
                IsModified = false;
                if (reset)
                    SetScenario(_clonedScenario, Applied);
                else
                {
                    _originalSenario = _clonedScenario; //crutch
                    Applied?.Invoke();
                }
            }
            catch (Exception e)
            {
                _log.ErrorFormat(e, "Ошибка инициализации сценария {0}", _clonedScenario.Name);
            }
        }

        public void Revert()
        {
            try
            {
                _clonedScenario = (ScenarioBase)Lazurite.Windows.Utils.Utils.CloneObject(_originalSenario);
                _clonedScenario.Initialize();
                buttonsView.Revert(_clonedScenario);
                _constructorView.Revert(_clonedScenario);
                IsModified = false;
            }
            catch (Exception e)
            {
                _log.ErrorFormat(e, "Ошибка инициализации сценария {0}", _clonedScenario.Name);
            }
        }
    }
}