using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.IOC;
using Lazurite.MainDomain;
using Lazurite.MainDomain.Statistics;
using Lazurite.Scenarios.ScenarioTypes;
using Lazurite.Shared;
using LazuriteUI.Windows.Controls;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace LazuriteUI.Windows.Main.Statistics.Views.DiagramViewImplementation
{
    /// <summary>
    /// Логика взаимодействия для SelectScenarioView.xaml
    /// </summary>
    public partial class SelectScenarioView : Grid
    {
        private static readonly ScenariosRepositoryBase ScenariosRepository = Singleton.Resolve<ScenariosRepositoryBase>();
        private static readonly IStatisticsManager StatisticsManager = Singleton.Resolve<IStatisticsManager>();
        
        public SelectScenarioView(string[] selectedScenarios)
        {
            InitializeComponent();

            itemsList.SelectionChanged += (o, e) => {
                var selection = itemsList.GetSelectedItems();
                if (selection.Length > 6)
                    selection[0].Selected = false;
            };

            btApply.Click += (o, e) => ApplyClicked?.Invoke(this, new EventsArgs<string[]>(SelectedIds));

            Loaded += async (o, e) =>
            {
                var targetScenarios =
                    ScenariosRepository.Scenarios
                    .Where(x =>
                        (x.ValueType is FloatValueType ||
                        x.ValueType is StateValueType ||
                        x.ValueType is ButtonValueType ||
                        x.ValueType is ToggleValueType ||
                        x.ValueType is InfoValueType ||
                        x.ValueType is DateTimeValueType))
                    .ToArray();

                var registrationInfo = await StatisticsManager.GetRegistrationInfo(targetScenarios);

                targetScenarios = targetScenarios.Where(x => registrationInfo.IsRegistered(x.Id) && (x.GetIsAvailable() || !(x is RemoteScenario))).ToArray();

                foreach (var scenario in targetScenarios)
                {
                    var item = new ItemView();
                    item.Icon = Icons.Icon.ChevronRight;
                    item.Selectable = true;
                    item.Content = scenario.Name.Length > 57 ? scenario.Name.Substring(0, 55) + "..." : scenario.Name;
                    item.Tag = scenario.Id;
                    item.Margin = new Thickness(2, 2, 2, 0);
                    item.Selected = selectedScenarios?.Contains(scenario.Id) ?? false;
                    itemsList.Children.Add(item);
                }

                if (targetScenarios.Any())
                    lblEmpty.Visibility = Visibility.Collapsed;
            };
        }

        public string[] SelectedIds => itemsList.GetSelectedItems().Select(x => (string)((ItemView)x).Tag).ToArray();

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event EventsHandler<string[]> ApplyClicked;

        public static void Show(string[] selectedScenarios, Action<string[]> callback)
        {
            var control = new SelectScenarioView(selectedScenarios);
            var dialog = new DialogView(control);
            control.ApplyClicked += (o, e) => {
                callback?.Invoke(e.Value);
                dialog.Close();
            };
            dialog.Show();
        }
    }
}
