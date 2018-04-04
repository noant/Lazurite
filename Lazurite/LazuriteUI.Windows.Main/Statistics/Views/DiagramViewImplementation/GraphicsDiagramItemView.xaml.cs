using System;
using System.Collections.Generic;
using System.Drawing;
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
            MainBrushProperty = DependencyProperty.Register(nameof(MainBrush), typeof(System.Windows.Media.Brush), typeof(GraphicsDiagramItemView), new PropertyMetadata(System.Windows.Media.Brushes.White));
            ScaleBrushProperty = DependencyProperty.Register(nameof(ScaleBrush), typeof(System.Windows.Media.Brush), typeof(GraphicsDiagramItemView), new PropertyMetadata(System.Windows.Media.Brushes.Yellow));
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

        public double Scroll { get; set; }

        public DateTime MaxDate { get; set; }
        public DateTime MinDate { get; set; }

        public StatisticsItem GetItemNear(DateTime dateTime)
        {
            var minDiff = double.MaxValue;
            StatisticsItem nearItem = null;
            foreach (var item in _items)
            {
                var diff = (item.DateTime - dateTime).TotalSeconds;
                if (diff >= 0 && diff < minDiff)
                {
                    minDiff = diff;
                    nearItem = item;
                }
            }
            return nearItem;
        }

        public void SelectPoint(DateTime dateTime)
        {
            throw new NotImplementedException();
        }

        public void SetPoints(StatisticsItem[] items)
        {
            _items = items;
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

        public void Refresh()
        {
            _totalSeconds = (int)(MaxDate - MinDate).TotalSeconds;
            _secondStart = (int)(_totalSeconds * Scroll);
            _secondEnd = _secondStart + (int)(_totalSeconds / Zoom);

            var itemsToDraw = _items.Where(x =>
            {
                var seconds = (MaxDate - x.DateTime).TotalSeconds;
                return _secondStart <= seconds && _secondEnd >= seconds;
            }).ToArray();

            DrawItems(itemsToDraw);
        }

        private void DrawItems(StatisticsItem[] statisticsItems)
        {
            var items = statisticsItems.ToList();

            gridMain.Children.Clear();

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

            if (items.Count == 1)
            {
                var point1 = Translate((int)(items[0].DateTime - MinDate).TotalSeconds, _values[items[0]]);

                var ellipse = new Ellipse
                {
                    Width = 3,
                    Height = 3,
                    Fill = MainBrush,
                    Margin = new Thickness(point1.X - 1, point1.Y - 1, 0, 0)
                };

                gridMain.Children.Add(ellipse);
            }
            else
                for (int i = 1; i < items.Count; i++)
                {
                    var item1 = items[i - 1];
                    var item2 = items[i];
                    var point1 = Translate((int)(item1.DateTime - MinDate).TotalSeconds, _values[item1]);
                    var point2 = Translate((int)(item2.DateTime - MinDate).TotalSeconds, _values[item2]);

                    var line = new Line
                    {
                        X1 = point1.X,
                        X2 = point2.X,
                        Y1 = point1.Y,
                        Y2 = point2.Y
                    };

                    line.Stroke = MainBrush;
                    line.StrokeThickness = 2;
                    line.VerticalAlignment = VerticalAlignment.Top;
                    line.HorizontalAlignment = HorizontalAlignment.Left;

                    gridMain.Children.Add(line);
                }
        }

        private PointD Translate(int second, double yVal)
        {
            var rangeX = _secondEnd - _secondStart;
            var kX = gridMain.ActualWidth / rangeX;
            var x = (second - _secondStart) * kX;

            var rangeY = _scaleYMax - _scaleYMin;
            var kY = gridMain.ActualHeight / rangeY;
            var y = gridMain.ActualHeight - ((yVal - _scaleYMin) * kY);

            return new PointD() {
                X = x,
                Y = y
            };
        }

        public void SetColors(System.Windows.Media.Brush mainColor, System.Windows.Media.Brush scaleColor)
        {
            MainBrush = mainColor;
            ScaleBrush = scaleColor;
        }

        public System.Windows.Media.Brush MainBrush
        {
            get => (System.Windows.Media.Brush)GetValue(MainBrushProperty);
            set => SetValue(MainBrushProperty, value);
        }

        public System.Windows.Media.Brush ScaleBrush
        {
            get => (System.Windows.Media.Brush)GetValue(ScaleBrushProperty);
            set => SetValue(ScaleBrushProperty, value);
        }

        public bool RequireLarge => true;

        private struct PointD
        {
            public double X;
            public double Y;
        }
    }
}