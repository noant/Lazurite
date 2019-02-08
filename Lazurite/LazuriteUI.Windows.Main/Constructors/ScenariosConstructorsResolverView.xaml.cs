using Lazurite.IOC;
using Lazurite.Logging;
using Lazurite.MainDomain;
using Lazurite.MainDomain.Statistics;
using Lazurite.Scenarios.ScenarioTypes;
using LazuriteUI.Windows.Controls;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace LazuriteUI.Windows.Main.Constructors
{
    /// <summary>
    /// Логика взаимодействия для ScenariosResolverView.xaml
    /// </summary>
    public partial class ScenariosConstructorsResolverView : UserControl
    {
        private static readonly ILogger Log = Singleton.Resolve<ILogger>();
        private static readonly ScenariosRepositoryBase Repository = Singleton.Resolve<ScenariosRepositoryBase>();
        private static readonly IStatisticsManager StatisticsManager = Singleton.Resolve<IStatisticsManager>();
        private IScenarioConstructorView _constructorView;
        private ScenarioBase _originalSenario;
        private ScenarioBase _clonedScenario;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event Action Applied;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event Action Modified;
        public bool IsModified { get; private set; }

        public ScenariosConstructorsResolverView()
        {
            InitializeComponent();
            buttonsView.ApplyClicked += () => Apply();
            buttonsView.ResetClicked += async () => await Revert();
            buttonsView.Modified += () => IsModified = true;
        }

        public async Task<bool> SetScenario(ScenarioBase scenario)
        {
            if (scenario != null)
            {
                var loading = StuckUILoadingWindow.Loading("Компоновка окна...");
                try
                {
                    _originalSenario = scenario;
                    _clonedScenario = (ScenarioBase)Lazurite.Windows.Utils.Utils.CloneObject(_originalSenario);
                    await _clonedScenario.Initialize();
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
                    return true;
                }
                catch (Exception e)
                {
                    EmptyScenarioModeOn();
                    Log.ErrorFormat(e, "Ошибка инициализации сценария {0}", scenario.Name);
                    return false;
                }
                finally
                {
                    loading.Close();
                }
            }
            else
            {
                EmptyScenarioModeOn();
                return false;
            }
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

        public async void Apply(Action callback = null, bool reset = true)
        {
            if (_originalSenario.ValueType != null && !_originalSenario.ValueType.IsCompatibleWith(_clonedScenario.ValueType))
            {
                MessageView.ShowYesNo(
                    "Сценарий был изменен так, что тип изначального сценария не совместим с типом текущего сценария.\r\n" +
                    "Если на этот сценарий ссылаются удаленные или комплексные сценарии, то они могут выполняться с ошибкой.\r\n" +
                    "Сохранить сценарий?",
                    "Сохранение сценария",
                    Icons.Icon.Warning,
                    async (result) =>
                    {
                        if (result)
                            await ApplyInternal(reset);
                        callback?.Invoke();
                    });
            }
            else
            {
                await ApplyInternal(reset);
                callback?.Invoke();
            }
        }

        private async Task ApplyInternal(bool reset = true)
        {
            try
            {
                Repository.SaveScenario(_clonedScenario);
                StatisticsManager.ReRegister(_clonedScenario);
                await _clonedScenario.Initialize();
                _clonedScenario.AfterInitilize();
                IsModified = false;
                if (reset)
                {
                    if (await SetScenario(_clonedScenario))
                        Applied?.Invoke();
                }
                else
                {
                    _originalSenario = _clonedScenario; //crutch
                    Applied?.Invoke();
                }
            }
            catch (Exception e)
            {
                Log.ErrorFormat(e, "Ошибка инициализации сценария {0}", _clonedScenario.Name);
            }
        }

        public async Task Revert()
        {
            try
            {
                _clonedScenario = (ScenarioBase)Lazurite.Windows.Utils.Utils.CloneObject(_originalSenario);
                await _clonedScenario.Initialize();
                buttonsView.Revert(_clonedScenario);
                _constructorView.Revert(_clonedScenario);
                IsModified = false;
            }
            catch (Exception e)
            {
                Log.ErrorFormat(e, "Ошибка инициализации сценария {0}", _clonedScenario.Name);
            }
        }
    }
}