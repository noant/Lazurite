using System;
using System.Collections.Generic;
using System.Data;
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

            Loaded += (o, e) => NeedItems?.Invoke(StatisticsFilter.Empty);
        }
        
        private DataTable CreateDataViews(StatisticsItem[] items)
        {
            var table = new DataTable();
            table.Columns.Add("ScenarioName");
            table.Columns.Add("DateTime", typeof(DateTime));
            table.Columns.Add("Value");
            table.Columns.Add("UserName");
            table.Columns.Add("SourceType");
            foreach (var item in items)
                table.Rows.Add(item.Target.Name, item.DateTime, item.Value, item.Source.Name, item.Source.SourceType);
            return table;
        }

        public void RefreshItems(StatisticsItem[] items)
        {
            if (items != null)
                gridControl.ItemsSource = CreateDataViews(items);
        }

        public Action<StatisticsFilter> NeedItems { get; set; }
    }
}