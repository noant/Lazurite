using Lazurite.IOC;
using Lazurite.MainDomain;
using Lazurite.Scenarios.ScenarioTypes;
using Lazurite.Windows.Logging;
using LazuriteUI.Icons;
using LazuriteUI.Windows.Controls;
using LazuriteUI.Windows.Main.Constructors;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace LazuriteUI.Windows.Main
{
    /// <summary>
    /// Логика взаимодействия для ScenariosConstructionView.xaml
    /// </summary>
    [LazuriteIcon(Icon.MovieClapperSelect)]
    [DisplayName("Конструктор сценариев")]
    public partial class ScenariosConstructionView : UserControl, IAllowSave, IDisposable
    {
        private ScenariosRepositoryBase _repository = Singleton.Resolve<ScenariosRepositoryBase>();
        private ScenarioBase _lastDeletedScenario;
        private WarningHandlerBase _warningHandler = Singleton.Resolve<WarningHandlerBase>();

        public ScenariosConstructionView()
        {
            InitializeComponent();
            switchesGrid.SelectedModelChanged += SwitchesGrid_SelectedModelChanged;
            switchesGrid.SelectedModelChanging += SwitchesGrid_SelectedModelChanging;
            switchesGrid.Initialize();

            constructorsResolver.Applied += () => switchesGrid.RefreshItemFull(constructorsResolver.GetScenario());
        }

        private void SwitchesGrid_SelectedModelChanging(Switches.ScenarioModel arg1, ScenarioChangingEventArgs args)
        {
            ThroughScenarioSave(args.Apply);
        }

        private void ThroughScenarioSave(Action callback)
        {
            if (constructorsResolver.GetScenario() != null && constructorsResolver.IsModified && _lastDeletedScenario != constructorsResolver.GetScenario())
            {
                switchesGrid.CancelDragging();
                MessageView.ShowYesNo(
                    "Сохранить изменения сценария [" + constructorsResolver.GetScenario().Name + "]?",
                    "Окно редактирования текущего сценария будет закрыто",
                    Icon.Save,
                    (result) =>
                    {
                        if (result)
                            constructorsResolver.Apply(() => callback?.Invoke(), false);
                        else
                            callback?.Invoke();
                    });
            }
            else
                callback?.Invoke();
        }

        private void SwitchesGrid_SelectedModelChanged(Switches.ScenarioModel obj)
        {
            constructorsResolver.SetScenario(switchesGrid.SelectedModel?.Scenario);
            btDeleteScenario.Visibility = switchesGrid.SelectedModel != null ? Visibility.Visible : Visibility.Collapsed;
        }

        private void btDeleteScenario_Click(object sender, RoutedEventArgs e)
        {
            MessageView.ShowYesNo("Вы уверены что хотите удалить выбранный сценарий?", "Удаление сценария", Icon.ListDelete,
                (result) => {
                    if (result)
                    {
                        var scenario = switchesGrid.SelectedModel.Scenario;
                        try
                        {
                            _repository.RemoveScenario(scenario);
                            _lastDeletedScenario = scenario;
                            switchesGrid.Remove(scenario);
                        }
                        catch (Exception exception)
                        {
                            MessageView.ShowMessage(exception.Message, "Невозможно удалить сценарий", Icon.Warning);
                            _warningHandler.Warn("Невозможно удалить сценарий", exception);
                        }
                    }
                }
            );
        }

        private void btCreateScenario_Click(object sender, RoutedEventArgs e)
        {
            ThroughScenarioSave(() => {
                var selectScenarioTypeControl = new NewScenarioSelectionView();
                var dialogView = new DialogView(selectScenarioTypeControl);
                dialogView.Show();

                selectScenarioTypeControl.SingleActionScenario += () => {
                    dialogView.Close();
                    NewScenario(new SingleActionScenario());
                };

                selectScenarioTypeControl.RemoteScenario += () => {
                    dialogView.Close();
                    NewScenario(new RemoteScenario());
                };

                selectScenarioTypeControl.CompositeScenario += () => {
                    dialogView.Close();
                    var selectCompositeScenarioType = new NewCompositeScenarioSelectionView();
                    var dialogViewComposite = new DialogView(selectCompositeScenarioType);
                    selectCompositeScenarioType.Selected += (valueType) => {
                        dialogViewComposite.Close();
                        var scenario = new CompositeScenario() { ValueType = valueType };
                        NewScenario(scenario);
                    };
                    dialogViewComposite.Show();
                };
            });
        }

        private void NewScenario(ScenarioBase newScenario)
        {
            newScenario.Initialize((res) => {
                if (res)
                    newScenario.AfterInitilize();
            });
            newScenario.Name = "Новый сценарий";
            _repository.AddScenario(newScenario);
            switchesGrid.Add(newScenario);
            constructorsResolver.SetScenario(newScenario);
        }

        public void Save(Action callback) => ThroughScenarioSave(callback);

        public void Dispose() => switchesGrid.Dispose();
    }
}
