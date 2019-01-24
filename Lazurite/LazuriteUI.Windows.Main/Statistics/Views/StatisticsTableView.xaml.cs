using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.IOC;
using Lazurite.Logging;
using Lazurite.MainDomain;
using Lazurite.MainDomain.Statistics;
using LazuriteUI.Windows.Controls;
using Syncfusion.Windows.Controls.Grid;
using Syncfusion.Windows.Controls.Grid.Converter;
using Syncfusion.XlsIO;
using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

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

        public StatisticsTableView()
        {
            InitializeComponent();
            gridControl.ColumnSizer = GridControlLengthUnitType.Star;

            Loaded += (o, e) => NeedItems?.Invoke(StatisticsFilter.Empty);
        }
        
        public void RefreshItems(StatisticsItem[] items, DateTime since, DateTime to)
        {
            if (items != null)
                gridControl.ItemsSource = CreateDataViews(items);
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

            public string ScenarioName { get; }
            public string UserName { get; }
            public DateTime DateTime { get; }
            public string Value { get; }
            public string SourceType { get; }
        }

        private void ExportClick(object sender, RoutedEventArgs e)
        {
            var sfd = new SaveFileDialog();
            sfd.Filter = "Таблица MS Excel (*.xlsx)|*.xlsx";
            sfd.FileName = "smarthome_data";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    gridControl.ExportToExcel(sfd.FileName, ExcelVersion.Excel2007);
                    MessageView.ShowYesNo(string.Format("Экспортировано успешно.\r\nОткрыть файл [{0}]?", sfd.FileName), "Экспорт", Icons.Icon.OfficeExcel,
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
}