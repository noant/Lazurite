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
    public partial class InfoDiagramItemView : Grid, IDiagramItem
    {
        public static readonly DependencyProperty MainBrushProperty;
        public static readonly DependencyProperty ScaleBrushProperty;

        static InfoDiagramItemView()
        {
            MainBrushProperty = DependencyProperty.Register(nameof(MainBrush), typeof(System.Windows.Media.SolidColorBrush), typeof(InfoDiagramItemView), new PropertyMetadata(System.Windows.Media.Brushes.SteelBlue));
            ScaleBrushProperty = DependencyProperty.Register(nameof(ScaleBrush), typeof(System.Windows.Media.SolidColorBrush), typeof(InfoDiagramItemView), new PropertyMetadata(System.Windows.Media.Brushes.Gray));
        }

        public InfoDiagramItemView()
        {
            InitializeComponent();
            SizeChanged += (o, e) => Refresh();
        }

        private StatisticsItem[] _items;
        private int _totalSeconds;
        private int _secondStart;
        private int _secondEnd;
        private int _scaleYMin = -1;
        private int _scaleYMax = 1;

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
            var p = item == null ? new Point() : Translate((int)(item.DateTime - MinDate).TotalSeconds, 0);
            if (!(p.Y == 0 && p.X == 0) && item != null && p.X != double.PositiveInfinity && p.X != double.NegativeInfinity && p.X != double.NaN)
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
            MaxDateCurrent = _items.Any() ? (DateTime?)_items.Last().DateTime : null;
            MinDateCurrent = _items.Any() ? (DateTime?)_items.First().DateTime : null;
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

            var points = 
                items
                .Select(x =>Translate((int)(x.DateTime - MinDate).TotalSeconds, 0))
                .ToArray();
            
            graphicsVisualHost.DrawPoints(points, MainBrush);
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