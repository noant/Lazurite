using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.IOC;
using Lazurite.Logging;
using Lazurite.MainDomain;
using Lazurite.MainDomain.Statistics;
using LazuriteUI.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using Xceed.Wpf.DataGrid;
using Xceed.Wpf.DataGrid.Export;
using Visual = LazuriteUI.Windows.Controls.Visual;

namespace LazuriteUI.Windows.Main.Statistics.Views
{
    /// <summary>
    /// Логика взаимодействия для StatisticsTableView.xaml
    /// </summary>
    public partial class StatisticsTableView : Grid, IStatisticsView
    {
        private static readonly ScenariosRepositoryBase ScenariosRepository = Singleton.Resolve<ScenariosRepositoryBase>();
        private static readonly string FloatValueTypeName = Lazurite.ActionsDomain.Utils.GetValueTypeClassName(typeof(FloatValueType));
        private static readonly ILogger Log = Singleton.Resolve<ILogger>();

        private StatisticItemView[] _items;

        public StatisticsTableView()
        {
            InitializeComponent();
            
            Loaded += (o, e) => NeedItems?.Invoke(StatisticsFilter.Empty);
            
            var _columnsSizeSetted = false;
            dataGrid.ItemsSourceChangeCompleted += (o, e) => {
                if (!_columnsSizeSetted)
                {
                    foreach (var column in dataGrid.VisibleColumns)
                        column.Width = 190;
                    _columnsSizeSetted = true;
                }
            };
        }
        
        public void RefreshItems(StatisticsItem[] items, DateTime since, DateTime to)
        {
            if (items != null)
                DataContext = new { Items = _items = CreateDataViews(items) };
        }

        private StatisticItemView[] CreateDataViews(StatisticsItem[] items)
        {
            var scenariosDictionary = ScenariosRepository.Scenarios.ToDictionary(x => x.Id);
            return items
                .Where(x => scenariosDictionary.ContainsKey(x.Target.ID))
                .Select
                (
                    x => {
                        if (x.Target.ValueTypeName == FloatValueTypeName)
                            return new StatisticItemView(x, ((FloatValueType)scenariosDictionary[x.Target.ID].ValueType).Unit);
                        else return new StatisticItemView(x);
                    }
                ).ToArray();
        }

        public Action<StatisticsFilter> NeedItems { get; set; }

        private class StatisticItemView
        {
            private static readonly string ToggleValueTypeName = Lazurite.ActionsDomain.Utils.GetValueTypeClassName(typeof(ToggleValueType));

            public StatisticItemView(StatisticsItem item, string scenarioUnit = "")
            {
                ScenarioName = item.Target.Name;
                UserName = item.Source.Name;
                SourceType = item.Source.SourceType;
                DateTime = item.DateTime;
                Value = item.Value + scenarioUnit;
                if (item.Target.ValueTypeName == ToggleValueTypeName)
                {
                    if (Value == ToggleValueType.ValueON)
                        Value = "Вкл.";
                    else
                        Value = "Выкл.";
                }
            }

            [Name("СЦЕНАРИЙ")]
            public string ScenarioName { get; }
            [Name("ДАТА И ВРЕМЯ")]
            public DateTime DateTime { get; }
            [Name("ЗНАЧЕНИЕ")]
            public string Value { get; }
            [Name("ПОЛЬЗОВАТЕЛЬ")]
            public string UserName { get; }
            [Name("ИСТОЧНИК")]
            public string SourceType { get; }
        }

        private void ExportClick(object sender, RoutedEventArgs e)
        {
            var sfd = new SaveFileDialog();
            sfd.Filter = "Файл таблицы CSV (*.csv)|*.csv";
            sfd.FileName = "Статистика_Lazurite";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (var writer = new StreamWriter(sfd.FileName, false, Encoding.UTF8))
                    using (var csv = new CsvWriter(writer))
                        csv.WriteRecords(_items);

                    MessageView.ShowYesNo($"Экспортировано успешно.\r\nОткрыть файл [{sfd.FileName}]?",  "Экспорт", Icons.Icon.Table,
                        (result) => {
                            if (result)
                                Process.Start(sfd.FileName);
                        });
                }
                catch (Exception exception)
                {
                    Log.Error(exception.Message, exception);
                }
            }
        }
    }

    /// <summary>
    /// Crutch for grid grouping (there is wrong background on GroupHeaderControl when grouped more than 1 columns)
    /// </summary>
    public class StackPanelCrutch : StackPanel
    {
        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            var parents = new List<DependencyObject>();
            GetAllParents(this, parents);

            foreach (var element in parents)
            {
                if (element is DataGridControl)
                    break;

                if (element is System.Windows.Controls.Panel p)
                    p.Background = Visual.BackgroundLazurite;

                if (element is Border b)
                    b.Padding = new Thickness(0);
            }
        }

        private void GetAllParents(DependencyObject child, List<DependencyObject> parents)
        {
            var parent = VisualTreeHelper.GetParent(child);
            if (parent != null)
            {
                parents.Add(parent);
                GetAllParents(parent, parents);
            }
        }
    }
}