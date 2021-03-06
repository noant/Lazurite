﻿using Lazurite.IOC;
using Lazurite.Logging;
using Lazurite.MainDomain;
using Lazurite.MainDomain.Statistics;
using Lazurite.Scenarios.ScenarioTypes;
using LazuriteUI.Icons;
using LazuriteUI.Windows.Controls;
using LazuriteUI.Windows.Main.Common;
using LazuriteUI.Windows.Main.Statistics.Settings;
using LazuriteUI.Windows.Main.Statistics.Views;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

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
        private StatisticsFilter _filter = StatisticsFilter.Empty;

        public StatisticsMainView()
        {
            InitializeComponent();

            listItems.SelectionChanged += ListItemsView_SelectionChanged;
            datesRangeView.SelectionChanged += async (o, e) => await Refresh();
            Loaded += async (o, args) =>
            {
                using (MessageView.ShowLoad("Загрузка информации о статистике..."))
                {
                    try
                    {
                        var registrationInfo = await StatisticsManager.GetRegistrationInfo(ScenariosRepository.Scenarios);

                        var registered =
                            await Task.WhenAll(
                                ScenariosRepository.Scenarios
                                .Where(x => (x.GetIsAvailable() || !(x is RemoteScenario)) && registrationInfo.IsRegistered(x.Id))
                                .Select(x => StatisticsManager.GetStatisticsInfoForScenario(x, SystemActionSource)));

                        datesRangeView.Min = registered.Any() ? registered.Min(x => x.Since) : DateTime.Now;
                        datesRangeView.Max = DateTime.Now;
                        datesRangeView.DateSelectionItem = new Common.DateSelectionItem(DateSelection.Last24Hours);
                    }
                    catch (Exception e)
                    {
                        Log.Error("Ошибка во время загрузки статистики", e);
                    }
                }
            };

            Unloaded += (o, e) => (_currentView as IDisposable)?.Dispose();
        }

        private void BtSettings_Click(object sender, RoutedEventArgs e)
        {
            StatisticsScenariosView.Show();
        }

        private void AppendView(IStatisticsView view)
        {
            if (_currentView?.GetType() != view.GetType())
            {
                _currentView = view;
                view.NeedItems = async (filter) =>
                {
                    _filter = filter;
                    await Refresh();
                };
                viewHostControl.Content = view;
            }
        }

        private async Task Refresh()
        {
            if (_currentView == null)
            {
                AppendView(new DiagramView());
            }
            else
            {
                var dateSince = datesRangeView.DateSelectionItem.Start;
                var dateTo = datesRangeView.DateSelectionItem.End;

                using (MessageView.ShowLoad("Загрузка информации о статистике..."))
                {
                    try
                    {
                        var registrationInfo = await StatisticsManager
                            .GetRegistrationInfo(
                                ScenariosRepository
                                .Scenarios
                                .Where(z =>
                                    _filter.All || (_filter.ScenariosIds?.Contains(z.Id) ?? false))
                                .ToArray());

                        var statisticScenariosInfos =
                            await Task.WhenAll(
                                ScenariosRepository
                                    .Scenarios
                                    .Where(x => (x.GetIsAvailable() || !(x is RemoteScenario)) && registrationInfo.IsRegistered(x.Id))
                                    .Select(x => StatisticsManager.GetStatisticsInfoForScenario(x, SystemActionSource)));

                        var items =
                            await Task.WhenAll(
                                statisticScenariosInfos
                                    .Select(x => StatisticsManager.GetItems(x, dateSince, dateTo, SystemActionSource)));

                        _currentView.RefreshItems(items, dateSince, dateTo);
                    }
                    catch (Exception e)
                    {
                        Log.Error("Ошибка во время загрузки статистики", e);
                    }
                }
            }
        }

        private void ListItemsView_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (listItems.SelectedItem == btTableView)
            {
                AppendView(new StatisticsTableView());
            }
            else if (listItems.SelectedItem == btPieView)
            {
                AppendView(new PieDiagramView());
            }
            else if (listItems.SelectedItem == btGeolocationView)
            {
                AppendView(new GeolocationView());
            }
            else
            {
                AppendView(new DiagramView());
            }
        }
    }
}