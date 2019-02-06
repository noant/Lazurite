using Lazurite.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LazuriteUI.Windows.Main.Statistics.Views.DiagramViewImplementation
{
    /// <summary>
    /// Логика взаимодействия для DiagramItemsHostView.xaml
    /// </summary>
    public partial class DiagramItemsHostView : Grid
    {
        public DiagramItemsHostView()
        {
            InitializeComponent();
            scrollBar.Scroll += (o, e) => {
                if (!_ingoreScrollEvent)
                {
                    Scroll = (int)e.NewValue;
                    Refresh();
                }
            };
        }

        private Dictionary<IDiagramItem, StatisticsItemView> _captions = new Dictionary<IDiagramItem, StatisticsItemView>();
        private IDiagramItem[] _items;

        public DateTime MinDate { get; set; }
        public DateTime MaxDate { get; set; }

        private bool _ingoreScrollEvent = false;
        private bool _dragMode = false;
        
        public void SetItems(IDiagramItem[] items)
        {
            _captions.Clear();
            _items = items;
            mainGrid.Children.Clear();
            mainGrid.RowDefinitions.Clear();

            if (_items.Any())
            {
                var index = 0;
                foreach (FrameworkElement item in _items)
                {
                    var rowDef = new RowDefinition();
                    rowDef.Height = new GridLength(((IDiagramItem)item).RequireLarge ? 2 : 1, GridUnitType.Star);
                    mainGrid.RowDefinitions.Add(rowDef);

                    mainGrid.Children.Add(item);
                    SetRow(item, index);

                    var captionView = new StatisticsItemView();
                    _captions.Add((IDiagramItem)item, captionView);
                    mainGrid.Children.Add(captionView);
                    SetRow(captionView, index);

                    index++;
                }
                BringScrollBarToZoom();
            }

            Refresh();
        }

        public void Refresh()
        {
            if (_items?.Any() ?? false)
            {
                var minDate = MinDate.AddSeconds(Scroll);
                var seconds = (MaxDate - MinDate).TotalSeconds / Zoom;
                var maxDate = minDate.AddSeconds(seconds);
                lblStart.Content = minDate.ToString();
                lblEnd.Content = maxDate.ToString();

                foreach (var item in _items)
                {
                    item.MaxDate = MaxDate;
                    item.MinDate = MinDate;

                    item.Zoom = Zoom;
                    item.Scroll = Scroll;

                    item.Refresh();
                }

                lblEnd.Visibility = Visibility.Visible;
                lblStart.Visibility = Visibility.Visible;
                line.Visibility = Visibility.Visible;
                lblSelectedTime.Visibility = Visibility.Visible;
                scrollBar.Visibility = Visibility.Visible;
                RefreshAddictionalInfo();
            }
            else
            {
                lblEnd.Visibility = Visibility.Collapsed;
                lblStart.Visibility = Visibility.Collapsed;
                scrollBar.Visibility = Visibility.Collapsed;
                line.Visibility = Visibility.Collapsed;
                lblSelectedTime.Visibility = Visibility.Collapsed;
            }
        }

        public double Zoom { get; set; } = 1;
        public int Scroll { get; set; } = 0;

        private void BtMagnifyMinus_Click(object sender, RoutedEventArgs e)
        {
            if (_items.Any())
            {
                Zoom *= 2;
                BringScrollBarToZoom();
                Refresh();
            }
        }

        private void BtMagnifyAdd_Click(object sender, RoutedEventArgs e)
        {
            if (_items.Any() && Zoom > 1)
            {
                Zoom /= 2;
                BringScrollBarToZoom();
                Refresh();
            }
        }

        private void BringScrollBarToZoom()
        {
            if (_items.Any())
            {
                _ingoreScrollEvent = true;
                var totalSeconds = (MaxDate - MinDate).TotalSeconds;
                scrollBar.Maximum = totalSeconds - totalSeconds / Zoom;
                scrollBar.Minimum = 0;

                if (Scroll > scrollBar.Maximum)
                    scrollBar.Value = Scroll = (int)scrollBar.Maximum;

                if (Zoom > 1)
                    scrollBar.Track.ViewportSize = scrollBar.Maximum / (Zoom - 1);
                else
                    scrollBar.Track.ViewportSize = double.NaN;

                _ingoreScrollEvent = false;
            }
        }

        private void HandleMouseClick(Point position)
        {
            var lineX = position.X;
            if (lineX < Constants.ScaleLeftMargin + Constants.DiagramsMargin.Left)
                lineX = Constants.ScaleLeftMargin + Constants.DiagramsMargin.Left;
            else if (lineX > mainGrid.ActualWidth - Constants.DiagramsMargin.Right)
                lineX = mainGrid.ActualWidth - Constants.DiagramsMargin.Right;
            RefreshAddictionalInfo(lineX);
        }

        private void RefreshAddictionalInfo(double lineLeftMargin = double.NaN)
        {
            if (double.IsNaN(lineLeftMargin))
                lineLeftMargin = line.Margin.Left;
            else
                line.Margin = new Thickness(lineLeftMargin, 0, 0, 0);

            var totalSeconds = (int)(MaxDate - MinDate).TotalSeconds;
            var secondStart = Scroll;
            var secondEnd = secondStart + (int)(totalSeconds / Zoom);

            var rangeX = secondEnd - secondStart;
            var kX = (mainGrid.ActualWidth - (Constants.ScaleLeftMargin + Constants.DiagramsMargin.Left + Constants.DiagramsMargin.Right)) / rangeX;
            var second = (lineLeftMargin - (Constants.ScaleLeftMargin + Constants.DiagramsMargin.Left)) / kX;

            var dateTime = MinDate.AddSeconds(Scroll + second);

            foreach (var captionPair in _captions)
            {
                captionPair.Key.SelectPoint(dateTime);
                var statisticsItem = captionPair.Key.GetItemNear(dateTime);
                captionPair.Value.Refresh(captionPair.Key.Points.ScenarioInfo, statisticsItem, dateTime);
                if (lineLeftMargin + captionPair.Value.ActualWidth > mainGrid.ActualWidth - Constants.DiagramsMargin.Left)
                    captionPair.Value.Margin = new Thickness(mainGrid.ActualWidth - Constants.DiagramsMargin.Left - captionPair.Value.ActualWidth, -5, 0, 0);
                else
                    captionPair.Value.Margin = new Thickness(lineLeftMargin, -5, 0, 0);
            }

            if (lineLeftMargin + lblSelectedTime.ActualWidth > mainGrid.ActualWidth - Constants.DiagramsMargin.Left)
                lblSelectedTime.Margin = new Thickness(mainGrid.ActualWidth - Constants.DiagramsMargin.Left - lblSelectedTime.ActualWidth, 0, 0, 0);
            else
                lblSelectedTime.Margin = new Thickness(lineLeftMargin, 0, 0, 0);

            lblSelectedTime.Content = dateTime.ToString();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var position = e.GetPosition(mainGrid);
            HandleMouseClick(position);
            _dragMode = true;
        }

        private void Grid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _dragMode = false;
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (_dragMode)
            {
                var position = e.GetPosition(mainGrid);
                HandleMouseClick(position);
            }
        }

        private void BtViewSettings_Click(object sender, RoutedEventArgs e)
        {
            ScenariosSelectPressed?.Invoke(this, new EventsArgs<object>(null));
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event EventsHandler<object> ScenariosSelectPressed;
    }
}