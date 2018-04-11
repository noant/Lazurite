using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Lazurite.MainDomain.Statistics;

namespace LazuriteUI.Windows.Main.Statistics.Views.DiagramViewImplementation
{
    /// <summary>
    /// Логика взаимодействия для GraphicsView.xaml
    /// </summary>
    public partial class StatesDiagramItemView : Grid, IDiagramItem
    {
        public static readonly DependencyProperty MainBrushProperty;
        public static readonly DependencyProperty ScaleBrushProperty;
        private static readonly Color OriginalLineColor = Colors.SteelBlue;
        private static Dictionary<int, Brush> BrushesCache = new Dictionary<int, Brush>();

        static StatesDiagramItemView()
        {
            MainBrushProperty = DependencyProperty.Register(nameof(MainBrush), typeof(System.Windows.Media.SolidColorBrush), typeof(StatesDiagramItemView), new PropertyMetadata(System.Windows.Media.Brushes.White));
            ScaleBrushProperty = DependencyProperty.Register(nameof(ScaleBrush), typeof(System.Windows.Media.SolidColorBrush), typeof(StatesDiagramItemView), new PropertyMetadata(System.Windows.Media.Brushes.Yellow));
        }

        public StatesDiagramItemView()
        {
            InitializeComponent();
            SizeChanged += (o, e) => Refresh();
        }

        private Dictionary<string, int> _valuesIds;
        private StatisticsItem[] _items;
        private int _totalSeconds;
        private int _secondStart;
        private int _secondEnd;
        private int _scaleYMin = -1;
        private int _scaleYMax;

        public double Zoom { get; set; }

        public DateTime? MaxDateCurrent { get; private set; }
        public DateTime? MinDateCurrent { get; private set; }

        public int Scroll { get; set; }

        public DateTime MaxDate { get; set; }
        public DateTime MinDate { get; set; }

        public StatisticsItem GetItemNear(DateTime dateTime)
        {
            return _items.LastOrDefault(x => x.DateTime <= dateTime);
        }

        public void SelectPoint(DateTime dateTime)
        {
            var item = GetItemNear(dateTime);
            var p = item == null ? new Point() : Translate((int)(item.DateTime - MinDate).TotalSeconds, _valuesIds[item.Value ?? string.Empty]);
            if (item != null && p.X != double.PositiveInfinity && p.X != double.NegativeInfinity && p.X != double.NaN)
            {
                ellipseSelectior.Visibility = Visibility.Visible;
                ellipseSelectior.Margin = new Thickness(p.X - 2, p.Y - 2, 0, 0);
            }
            else
                ellipseSelectior.Visibility = Visibility.Collapsed;
        }
        
        public void SetPoints(string scenarioName, StatisticsItem[] items)
        {
            var values = items.Select(x => x.Value ?? string.Empty).Distinct().OrderBy(x => x).ToList();
            _valuesIds = Enumerable.Range(0, values.Count).ToDictionary(x => values[x]);

            lblScenName.Content = scenarioName;

            _items = items.OrderBy(x=>x.DateTime).ToArray();
            MaxDateCurrent = _items.Any() ? (DateTime?)_items.Max(x => x.DateTime) : null;
            MinDateCurrent = _items.Any() ? (DateTime?)_items.Min(x => x.DateTime) : null;
            _scaleYMin = -1;
            _scaleYMax = _valuesIds.Count + 1;
        }

        // =((((
        public void Refresh()
        {
            _totalSeconds = (int)(MaxDate - MinDate).TotalSeconds;
            _secondStart = Scroll;
            _secondEnd = _secondStart + (int)(_totalSeconds / Zoom);

            var itemsToDraw = _items.Where(x =>
            {
                var seconds = (x.DateTime - MinDate).TotalSeconds;
                return _secondStart <= seconds && _secondEnd >= seconds;
            }).ToList();

            //take one previous and one following items (to prevent diagram winking)
            var firstItemDateTime = _items
                .Where(x => (x.DateTime - MinDate).TotalSeconds < _secondStart)
                .OrderByDescending(x => x.DateTime);
            var lastItemDateTime = _items
                .Where(x => (x.DateTime - MinDate).TotalSeconds > _secondEnd)
                .OrderBy(x => x.DateTime);
            var item1 = firstItemDateTime.FirstOrDefault();
            if (item1 != null)
                itemsToDraw.Add(item1);
            var item2 = lastItemDateTime.FirstOrDefault();
            if (item2 != null)
                itemsToDraw.Add(item2);

            DrawItems(itemsToDraw.OrderBy(x => x.DateTime).ToArray());
        }

        private void DrawItems(StatisticsItem[] statisticsItems)
        {
            var items = statisticsItems;

            var brushedPoints = 
                items
                .Select(x =>
                    {
                        var index = _valuesIds[x.Value ?? string.Empty];
                        var brush = GetBrush(index);
                        return new
                        {
                            Point = Translate((int)(x.DateTime - MinDate).TotalSeconds, index),
                            Brush = brush
                        };
                    })
                .ToList();
                        
            if (brushedPoints.Count > 0)
            {
                var last = brushedPoints.Last();
                var now = Translate(_secondEnd + 86400, 0);
                now.Y = last.Point.Y;

                brushedPoints.Add(new { Point = now, Brush = last.Brush });

                var lines = new List<ColoredLine>();
                
                for (int i = 0; i < brushedPoints.Count - 1; i++)
                {
                    var brushedPointStart = brushedPoints[i];
                    var pointEnd = new Point(brushedPoints[i + 1].Point.X, brushedPointStart.Point.Y);
                    lines.Add(new ColoredLine()
                    {
                        Point1 = brushedPointStart.Point,
                        Point2 = pointEnd,
                        Brush = brushedPointStart.Brush
                    });
                }
                
                graphicsVisualHost.DrawLines(lines);
            }
        }

        private Brush GetBrush(int index)
        {
            if (BrushesCache.ContainsKey(index))
                return BrushesCache[index];
            else
            {
                var blueOriginal = OriginalLineColor.B;
                var blueIncrement = blueOriginal - ((blueOriginal / 12) * index);
                if (blueIncrement < 0)
                    blueIncrement = 0;
                var brush = new SolidColorBrush(Color.FromRgb(OriginalLineColor.R, OriginalLineColor.G, (byte)blueIncrement));
                BrushesCache.Add(index, brush);
                return brush;
            }
        }

        private Point Translate(int second, double yVal)
        {
            return Utils.Translate(second, yVal, _secondStart, _secondEnd, _scaleYMin, _scaleYMax, gridMain.ActualWidth, gridMain.ActualHeight);
        }

        public void SetColors(SolidColorBrush mainColor, SolidColorBrush scaleColor)
        {
            MainBrush = mainColor;
            ScaleBrush = scaleColor;
        }

        public SolidColorBrush MainBrush
        {
            get => (SolidColorBrush)GetValue(MainBrushProperty);
            set => SetValue(MainBrushProperty, value);
        }

        public SolidColorBrush ScaleBrush
        {
            get => (SolidColorBrush)GetValue(ScaleBrushProperty);
            set => SetValue(ScaleBrushProperty, value);
        }

        public bool RequireLarge => false;
    }
}