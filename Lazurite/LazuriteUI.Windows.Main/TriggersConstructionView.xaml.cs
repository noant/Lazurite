using Lazurite.ActionsDomain;
using Lazurite.CoreActions;
using Lazurite.IOC;
using Lazurite.MainDomain;
using Lazurite.Scenarios.ScenarioTypes;
using Lazurite.Windows.Logging;
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
    [LazuriteIcon(Icon.TimerForward)]
    [DisplayName("Триггеры")]
    public partial class TriggersConstructionView : UserControl
    {
        private ScenariosRepositoryBase _repository = Singleton.Resolve<ScenariosRepositoryBase>();
        private Lazurite.MainDomain.TriggerBase _lastDeletedTrigger;
        private WarningHandlerBase _warningHandler = Singleton.Resolve<WarningHandlerBase>();

        public TriggersConstructionView()
        {
            InitializeComponent();
            this.triggersListView.SelectionChanged += TriggersList_SelectionChanged;
            this.triggersListView.SelectionChanging += TriggersList_SelectionChanging;

            this.constructorsResolver.Applied += () => this.triggersListView.Refresh(this.constructorsResolver.GetTrigger());
        }

        private void TriggersList_SelectionChanging(Lazurite.MainDomain.TriggerBase arg1, TriggerChangingEventArgs args)
        {
            ThroughTriggerSave(args.Apply);
        }

        private void ThroughTriggerSave(Action callback)
        {
            if (this.constructorsResolver.GetTrigger() != null && this.constructorsResolver.IsModified && _lastDeletedTrigger != this.constructorsResolver.GetTrigger())
            {
                MessageView.ShowYesNo(
                    "Сохранить изменения триггера [" + this.constructorsResolver.GetTrigger().Name + "]?",
                    "Окно редактирования текущего триггера будет закрыто",
                    Icon.Save,
                    (result) =>
                    {
                        if (result)
                            constructorsResolver.Apply(() => callback?.Invoke());
                        else
                            callback?.Invoke();
                    });
            }
            else
                callback?.Invoke();
        }

        private void TriggersList_SelectionChanged(Lazurite.MainDomain.TriggerBase obj)
        {
            this.constructorsResolver.SetTrigger(this.triggersListView.SelectedTrigger);
            btDeleteTrigger.Visibility = this.triggersListView.SelectedTrigger != null ? Visibility.Visible : Visibility.Collapsed;
        }

        private void btDeleteScenario_Click(object sender, RoutedEventArgs e)
        {
            MessageView.ShowYesNo("Вы уверены, что хотите удалить выбранный триггер?", "Удаление триггера", Icon.ListDelete,
                (result) => {
                    if (result)
                    {
                        var trigger = this.triggersListView.SelectedTrigger;
                        try
                        {
                            _repository.RemoveTrigger(trigger);
                            _lastDeletedTrigger = trigger;
                            this.triggersListView.Remove(trigger);
                        }
                        catch (Exception exception)
                        {
                            MessageView.ShowMessage(exception.Message, "Невозможно удалить триггер", Icon.Warning);
                            _warningHandler.Warn("Невозможно удалить триггер", exception);
                        }
                    }
                }
            );
        }

        private void btCreateScenario_Click(object sender, RoutedEventArgs e)
        {
            ThroughTriggerSave(() => {
                var trigger = new Lazurite.Scenarios.TriggerTypes.Trigger();
                trigger.Name = "Новый триггер";
                _repository.AddTrigger(trigger);
                this.triggersListView.Add(trigger);
                this.constructorsResolver.SetTrigger(trigger);
            });
        }
    }
}
