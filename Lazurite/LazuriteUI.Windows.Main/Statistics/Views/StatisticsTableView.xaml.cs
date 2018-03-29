using System;
using System.Collections.Generic;
using System.Globalization;
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
using Lazurite.MainDomain.Statistics;
using Syncfusion.Windows.Controls.Grid;

namespace LazuriteUI.Windows.Main.Statistics.Views
{
    /// <summary>
    /// Логика взаимодействия для StatisticsTableView.xaml
    /// </summary>
    public partial class StatisticsTableView : Grid, IStatisticsView
    {
        public StatisticsTableView()
        {
            InitializeComponent();
            gridControl.ColumnSizer = GridControlLengthUnitType.Star;
            Loaded += StatisticsTableView_Loaded;
        }

        private void StatisticsTableView_Loaded(object sender, RoutedEventArgs e)
        {
            var items = NeedItems?.Invoke(StatisticsFilter.Empty);
            if (items != null)
            {
                gridControl.ItemsSource = CreateDataViews(items);
            }
        }

        private StatisticItemView[] CreateDataViews(StatisticsItem[] items)
        {
            return items.Select(x => new StatisticItemView() {
                DateTime = x.DateTime,
                ScenarioName = x.Target.Name,
                SourceType = x.Source?.SourceType ?? "Система",
                UserName = x.Source?.Name ?? "Системный пользователь",
                Value = x.Value
            }).ToArray();
        }

        public Func<StatisticsFilter, StatisticsItem[]> NeedItems { get; set; }

        private class StatisticItemView
        {
            public string ScenarioName { get; set; }
            public string UserName { get; set; }
            public DateTime DateTime { get; set; }
            public string Value { get; set; }
            public string SourceType { get; set; }
        }
    }
}