using Lazurite.IOC;
using Lazurite.Logging;
using Lazurite.MainDomain;
using Lazurite.MainDomain.Statistics;
using Lazurite.Utils;
using LazuriteUI.Icons;
using LazuriteUI.Windows.Controls;
using LazuriteUI.Windows.Main.Common;
using LazuriteUI.Windows.Main.Statistics.Settings;
using LazuriteUI.Windows.Main.Statistics.Views;
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

namespace LazuriteUI.Windows.Main.Statistics
{
    /// <summary>
    /// Логика взаимодействия для StatisticsMainView.xaml
    /// </summary>
    [LazuriteIcon(Icon.GraphLine)]
    [DisplayName("Статистика")]
    public partial class StatisticsMainView : Grid
    {
        private static readonly IStatisticsManager StatisticsManager = Singleton.Resolve<IStatisticsManager>();
        private static readonly ScenariosRepositoryBase ScenariosRepository = Singleton.Resolve<ScenariosRepositoryBase>();
        private static readonly UsersRepositoryBase UsersRepository = Singleton.Resolve<UsersRepositoryBase>();
        private static readonly ScenarioActionSource SystemActionSource = new ScenarioActionSource(UsersRepository.SystemUser, ScenarioStartupSource.System, ScenarioAction.ViewValue);
        private static readonly ILogger Log = Singleton.Resolve<ILogger>();

        private IStatisticsView _currentView;
        private string[] _selectedScenariosIds;

        public StatisticsMainView()
        {
            InitializeComponent();
            
            listItems.SelectionChanged += ListItemsView_SelectionChanged;
            datesRangeView.SelectionChanged += (o, e) => Refresh();
            Loaded += (o, args) =>
            {
                StuckUILoadingWindow.Show("Загрузка информации...", 
                    () => {
                        try
                        {
                            var registered = ScenariosRepository.Scenarios
                                .Where(x => StatisticsManager.IsRegistered(x))
                                .Select(x => StatisticsManager.GetStatisticsInfoForScenario(x, SystemActionSource))
                                .ToArray();
                            datesRangeView.Min = registered.Any() ? registered.Min(x => x.Since) : DateTime.Now;
                            datesRangeView.Max = DateTime.Now;
                            datesRangeView.DateSelectionItem = new Common.DateSelectionItem(DateSelection.Last24Hours);
                        }
                        catch (Exception e)
                        {
                            Log.Error("Ошибка во время загрузки статистики", e);
                        }
                    }
                );
            };
        }
        
        private void btSettings_Click(object sender, RoutedEventArgs e)
        {
            StatisticsScenariosView.Show();
        }
        
        private void AppendView(IStatisticsView view)
        {
            if (_currentView?.GetType() != view.GetType())
            {
                _currentView = view;
                view.NeedItems = (filter) =>
                {
                    _selectedScenariosIds = filter.ScenariosIds;
                    Refresh();
                };
                viewHostControl.Content = view;
            }
        }

        private void Refresh()
        {
            if (_currentView == null)
                AppendView(new DiagramView());
            else
            {
                bool selectionEmpty = _selectedScenariosIds == null || !_selectedScenariosIds.Any();
                var dateSince = datesRangeView.DateSelectionItem.Start;
                var dateTo = datesRangeView.DateSelectionItem.End;

                StuckUILoadingWindow.Show("Загрузка информации...",
                    () => {
                        try
                        {
                            var items = ScenariosRepository
                                .Scenarios
                                .Where(x => StatisticsManager.IsRegistered(x) && (selectionEmpty || _selectedScenariosIds.Contains(x.Id)))
                                .Select(x => StatisticsManager.GetStatisticsInfoForScenario(x, SystemActionSource))
                                .SelectMany(x => StatisticsManager.GetItems(x, dateSince, dateTo, SystemActionSource))
                                .OrderByDescending(x => x.DateTime)
                                .ToArray();
                            _currentView.RefreshItems(items, dateSince, dateTo);
                        }
                        catch (Exception e)
                        {
                            Log.Error("Ошибка во время загрузки статистики", e);
                        }
                    }
                );
            }
        }

        private void ListItemsView_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (listItems.SelectedItem == btTableView)
                AppendView(new StatisticsTableView());
            else if (listItems.SelectedItem == btPieView)
                AppendView(new PieDiagramView());
            else AppendView(new DiagramView());
        }
    }
}
