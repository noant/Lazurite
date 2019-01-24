using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace LazuriteUI.Windows.Main.Statistics.Views.PieDiagramViewImplementation
{
    /// <summary>
    /// Логика взаимодействия для PieChartView.xaml
    /// </summary>
    public partial class PieChartView : Grid
    {
        private const int MaxCount = 10;
        public static readonly DependencyProperty ContentProperty;

        static PieChartView()
        {
            ContentProperty = DependencyProperty.Register(nameof(Content), typeof(object), typeof(PieChartView), new FrameworkPropertyMetadata()
            {
                PropertyChangedCallback = (o, e) =>
                {
                    ((PieChartView)o).lblCaption.Content = e.NewValue;
                }
            });
        }

        public PieChartView()
        {
            InitializeComponent();
        }

        public object Content
        {
            get => (string)GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }

        public void SetItems(StatisticsCategoryView[] views)
        {
            var total = views.Sum(x => x.Weight);
            var items = views.Select(x => new StatisticsCategoryViewInternal()
                {
                    Category = x.Category,
                    Count = x.Weight,
                    Percentage = x.Weight / (double)total
                })
                .ToArray();

            if (items.Length > MaxCount)
            {
                var ordered = items.OrderBy(x => x.Count).ToArray();
                var first9 = ordered.Take(MaxCount - 1).ToList();
                var last = ordered.Except(first9).ToArray();
                var itemLast = new StatisticsCategoryViewInternal() {
                    Category = "Другое",
                    Count = last.Sum(x => x.Count),
                    Percentage = last.Sum(x => x.Percentage)
                };
                first9.Add(itemLast);
                items = first9.ToArray();
            }
            chart.ItemsSource = null; //crutch
            chart.ItemsSource = items;
        }

        private class StatisticsCategoryViewInternal
        {
            public string Category { get; set; }
            public double Percentage { get; set; }
            public int Count { get; set; }

            public string Description
            {
                get => string.Format("{0}% ({1})", Math.Round(Percentage * 100, 2), Count);
            }
        }
    }

    public class StatisticsCategoryView
    {
        public string Category { get; set; }
        public int Weight { get; set; }
    }
}
