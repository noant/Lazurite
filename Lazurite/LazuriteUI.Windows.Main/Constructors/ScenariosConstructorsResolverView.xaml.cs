using Lazurite.IOC;
using Lazurite.Logging;
using Lazurite.MainDomain;
using Lazurite.MainDomain.Statistics;
using Lazurite.Scenarios.ScenarioTypes;
using LazuriteUI.Windows.Controls;
using System;
using System.Linq;
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
        public bool IsFailed { get; private set; }

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
                using (StuckUILoadingWindow.Loading("Компоновка окна..."))
                {
                    try
                    {
                        _originalSenario = scenario;
                        _clonedScenario = (ScenarioBase)Lazurite.Windows.Utils.Utils.CloneObject(_originalSenario);
                        if (_clonedScenario is SingleActionScenario s)
                        {
                            await _clonedScenario.Initialize();
                            contentPresenter.Content = _constructorView = new SingleActionScenarioView(s);
                        }
                        else if (_clonedScenario is CompositeScenario c)
                        {
                            await _clonedScenario.Initialize();
                            contentPresenter.Content = _constructorView = new CompositeScenarioView(c);
                        }
                        else if (_clonedScenario is RemoteScenario r)
                        {
                            contentPresenter.Content = _constructorView = new RemoteScenarioView(r);
                        }

                        buttonsView.SetScenario(_clonedScenario);
                        IsModified = false;
                        _constructorView.Modified += () =>
                        {
                            Modified?.Invoke();
                            buttonsView.ScenarioModified();
                            IsModified = true;
                        };
                        _constructorView.Failed += () =>
                        {
                            buttonsView.Failed();
                            IsFailed = true;
                        };
                        _constructorView.Succeed += () =>
                        {
                            buttonsView.Success();
                            IsFailed = false;
                        };
                        EmptyScenarioModeOff();
                        return true;
                    }
                    catch (Exception e)
                    {
                        EmptyScenarioModeOn();
                        Log.Error($"Ошибка инициализации сценария [{scenario.Name}]", e);
                        return false;
                    }
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
            if (Repository.Scenarios.Any(x => x.Id == _originalSenario.Id) &&
                _originalSenario.ValueType != null &&
                !_originalSenario.ValueType.IsCompatibleWith(_clonedScenario.ValueType))
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
                        {
                            await ApplyInternal(reset);
                        }

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
                using (MessageView.ShowLoad("Применение изменений..."))
                {
                    Repository.SaveScenario(_clonedScenario);
                    StatisticsManager.ReRegister(_clonedScenario);
                    await _clonedScenario.Initialize();
                    _clonedScenario.AfterInitilize();
                }

                IsModified = false;

                if (reset)
                {
                    if (await SetScenario(_clonedScenario))
                    {
                        Applied?.Invoke();
                    }
                }
                else
                {
                    _originalSenario = _clonedScenario; //crutch
                    Applied?.Invoke();
                }
            }
            catch (Exception e)
            {
                Log.Error($"Ошибка инициализации сценария [{_clonedScenario.Name}]", e);
            }
        }

        public async Task Revert()
        {
            try
            {
                _clonedScenario = (ScenarioBase)Lazurite.Windows.Utils.Utils.CloneObject(_originalSenario);

                if (_clonedScenario is RemoteScenario == false)
                {
                    await _clonedScenario.Initialize();
                }

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