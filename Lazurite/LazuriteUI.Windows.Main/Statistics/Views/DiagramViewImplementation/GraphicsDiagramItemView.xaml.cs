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
    public partial class GraphicsDiagramItemView : Grid, IDiagramItem
    {
        public static readonly DependencyProperty MainBrushProperty;
        public static readonly DependencyProperty ScaleBrushProperty;

        static GraphicsDiagramItemView()
        {
            MainBrushProperty = DependencyProperty.Register(nameof(MainBrush), typeof(System.Windows.Media.SolidColorBrush), typeof(GraphicsDiagramItemView), new PropertyMetadata(System.Windows.Media.Brushes.SteelBlue));
            ScaleBrushProperty = DependencyProperty.Register(nameof(ScaleBrush), typeof(System.Windows.Media.SolidColorBrush), typeof(GraphicsDiagramItemView), new PropertyMetadata(System.Windows.Media.Brushes.Gray));
        }

        public GraphicsDiagramItemView()
        {
            InitializeComponent();
            SizeChanged += (o, e) => Refresh();
        }

        private StatisticsItem[] _items;
        private Dictionary<StatisticsItem, double> _values;
        private double _scaleYMin;
        private double _scaleYMax;
        private int _totalSeconds;
        private int _secondStart;
        private int _secondEnd;
        private double _realYMin;

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
            var p = item == null ? new Point() : Translate((int)(item.DateTime - MinDate).TotalSeconds, _values[item]);
            if (item != null && p.X != double.PositiveInfinity && p.X != double.NegativeInfinity && p.X != double.NaN)
            {
                ellipseSelectior.Visibility = Visibility.Visible;
                ellipseSelectior.Margin = new Thickness(p.X - 3, p.Y - 3, 0, 0);
            }
            else
                ellipseSelectior.Visibility = Visibility.Collapsed;
        }

        public void SetPoints(string scenarioName, StatisticsItem[] items)
        {
            lblScenName.Content = scenarioName;

            _items = items.OrderBy(x=>x.DateTime).ToArray();
            MaxDateCurrent = _items.Any() ? (DateTime?)_items.Max(x => x.DateTime) : null;
            MinDateCurrent = _items.Any() ? (DateTime?)_items.Min(x => x.DateTime) : null;
            _values = new Dictionary<StatisticsItem, double>();
            foreach(var item in _items)
                _values.Add(item, double.Parse(item.Value));
            var min = _realYMin = _values.Any() ? _values.Min(x => x.Value) : 0;
            var max = _values.Any() ? _values.Max(x => x.Value) : 0;
            var scaleIncrease = (max - min) / 5;
            if (scaleIncrease == 0)
                scaleIncrease = max / 5;
            if (scaleIncrease == 0)
                scaleIncrease = 1;
            _scaleYMin = Math.Round((min - scaleIncrease) - 0.5, 0);
            _scaleYMax = Math.Round((max + scaleIncrease) + 0.5, 0);
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

            var scaleDiff = (_scaleYMax - _scaleYMin) / 4;
            lblScaleMin.Content = _scaleYMin;
            lblScaleMax.Content = _scaleYMax;

            lineX.Margin = new Thickness(-11, Translate(0, 0).Y, 0, 0);
            lblScaleZero.Margin = new Thickness(0, lineX.Margin.Top, 11, 0);

            if (lineX.Margin.Top >= gridMain.ActualHeight - 30)
            {
                lblScaleMin.Visibility = Visibility.Collapsed;
                lblScaleMax.Visibility = Visibility.Visible;
            }
            else if (lineX.Margin.Top <= 30)
            {
                lblScaleMin.Visibility = Visibility.Visible;
                lblScaleMax.Visibility = Visibility.Collapsed;
            }
            else
            {
                lblScaleMin.Visibility = Visibility.Visible;
                lblScaleMax.Visibility = Visibility.Visible;
            }

            var points = 
                items
                .Select(x => Translate((int)(x.DateTime - MinDate).TotalSeconds, _values[x]))
                .ToList();
                        
            if (points.Count > 0)
            {
                var now = Translate(_secondEnd + 86400, 0);
                now.Y = points.Last().Y;

                points.Add(now);

                var lines = new List<Line>();
                
                for (int i = 1; i < points.Count; i++)
                {
                    lines.Add(new Line()
                    {
                        Point1 = points[i - 1],
                        Point2 = points[i]
                    });
                }
                
                graphicsVisualHost.DrawLines(lines, MainBrush);
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

        public bool RequireLarge => true;
    }
}