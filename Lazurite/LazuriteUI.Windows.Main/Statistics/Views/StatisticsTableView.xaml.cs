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
using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.IOC;
using Lazurite.MainDomain;
using Lazurite.MainDomain.Statistics;
using Syncfusion.Windows.Controls.Grid;

namespace LazuriteUI.Windows.Main.Statistics.Views
{
    /// <summary>
    /// Логика взаимодействия для StatisticsTableView.xaml
    /// </summary>
    public partial class StatisticsTableView : Grid, IStatisticsView
    {
        private static readonly ScenariosRepositoryBase ScenariosRepository = Singleton.Resolve<ScenariosRepositoryBase>();
        private static readonly string FloatValueTypeName = Lazurite.ActionsDomain.Utils.GetValueTypeClassName(typeof(FloatValueType));

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

    }
}