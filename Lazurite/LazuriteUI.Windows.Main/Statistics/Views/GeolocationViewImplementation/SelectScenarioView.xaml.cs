using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.IOC;
using Lazurite.MainDomain;
using Lazurite.MainDomain.Statistics;
using Lazurite.Shared;
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

namespace LazuriteUI.Windows.Main.Statistics.Views.GeolocationViewImplementation
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

            var targetScenarios =
                ScenariosRepository.Scenarios
                .Where(x => 
                    StatisticsManager.IsRegistered(x) &&
                    x.ValueType is GeolocationValueType)
                .ToArray();

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
            
            btApply.Click += (o, e) => ApplyClicked?.Invoke(this, new EventsArgs<string[]>(SelectedIds));
        }

        public string[] SelectedIds
        {
            get => itemsList.GetSelectedItems().Select(x => (string)((ItemView)x).Tag).ToArray();
        }

        public event EventsHandler<string[]> ApplyClicked;

        public static void Show(string[] selectedScenarios, Action<string[]> callback, Action dialogClosed)
        {
            var control = new SelectScenarioView(selectedScenarios);
            var dialog = new DialogView(control);
            control.ApplyClicked += (o, e) => {
                callback?.Invoke(e.Value);
                dialog.Close();
            };
            dialog.Closed += (o, e) => dialogClosed?.Invoke();
            dialog.Show();
        }
    }
}
