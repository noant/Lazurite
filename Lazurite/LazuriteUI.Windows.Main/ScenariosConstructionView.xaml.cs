using Lazurite.ActionsDomain;
using Lazurite.CoreActions;
using Lazurite.IOC;
using Lazurite.MainDomain;
using Lazurite.Scenarios.ScenarioTypes;
using LazuriteUI.Icons;
using LazuriteUI.Windows.Controls;
using LazuriteUI.Windows.Main.Constructors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace LazuriteUI.Windows.Main
{
    /// <summary>
    /// Логика взаимодействия для ScenariosConstructionView.xaml
    /// </summary>
    [LazuriteIcon(Icon.MovieClapperSelect)]
    [DisplayName("Конструктор сценариев")]
    public partial class ScenariosConstructionView : UserControl
    {
        private ScenariosRepositoryBase _repository = Singleton.Resolve<ScenariosRepositoryBase>();

        public ScenariosConstructionView()
        {
            InitializeComponent();
            this.switchesGrid.SelectedModelChanged += SwitchesGrid_SelectedModelChanged;
            this.switchesGrid.SelectedModelChanging += SwitchesGrid_SelectedModelChanging;
            this.switchesGrid.Initialize();

            this.constructorsResolver.Applied += () => this.switchesGrid.RefreshItemFull(this.constructorsResolver.GetScenario());
        }

        private void SwitchesGrid_SelectedModelChanging(Switches.ScenarioModel arg1, ScenarioChangingEventArgs args)
        {
            if (this.constructorsResolver.GetScenario() != null && this.constructorsResolver.IsModified)
            {
                switchesGrid.CancelDragging();
                MessageView.ShowYesNo(
                    "Сохранить изменения сценария [" + this.constructorsResolver.GetScenario().Name + "]?",
                    "Выбран другой сценарий",
                    Icon.Save,
                    (result) =>
                    {
                        if (result)
                            constructorsResolver.Apply(() => args.Apply());
                        else
                            args.Apply();
                    });
            }
            else args.Apply();
        }

        private void SwitchesGrid_SelectedModelChanged(Switches.ScenarioModel obj)
        {
            this.constructorsResolver.SetScenario(this.switchesGrid.SelectedModel?.Scenario);
        }

        private void btDeleteScenario_Click(object sender, RoutedEventArgs e)
        {
            MessageView.ShowYesNo("Вы уверены, что хотите удалить выбранный сценарий?", "Удаление сценария", Icon.ListDelete,
                (result) => {
                    if (result)
                    {
                        var scenario = this.switchesGrid.SelectedModel.Scenario;
                        this.switchesGrid.Remove(scenario);
                        _repository.RemoveScenario(scenario);
                    }
                }
            );
        }

        private void btCreateScenario_Click(object sender, RoutedEventArgs e)
        {
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
                    if (valueType.AcceptedValues.Any())
                        scenario.InitializeWithValue = valueType.AcceptedValues[0];
                    NewScenario(scenario);
                };
                dialogViewComposite.Show();
            };
        }

        private void NewScenario(ScenarioBase newScenario)
        {
            newScenario.Name = "Новый сценарий";
            _repository.AddScenario(newScenario);
            this.switchesGrid.Add(newScenario, null);
            this.constructorsResolver.SetScenario(newScenario);
        }
    }
}
