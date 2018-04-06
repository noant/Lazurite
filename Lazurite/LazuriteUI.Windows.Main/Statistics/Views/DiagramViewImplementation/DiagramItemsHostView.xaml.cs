using System;
using System.Collections.Generic;
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

        private IDiagramItem[] _items;

        DateTime? _minDate;
        DateTime? _maxDate;

        private bool _ingoreScrollEvent = false;

        public void SetItems(IDiagramItem[] items)
        {
            _items = items;
            mainGrid.Children.Clear();
            mainGrid.RowDefinitions.Clear();

            if (items.Any())
            {
                var index = 0;
                foreach (FrameworkElement item in items)
                {
                    var rowDef = new RowDefinition();
                    rowDef.Height = new GridLength(((IDiagramItem)item).RequireLarge ? 2 : 1, GridUnitType.Star);
                    mainGrid.RowDefinitions.Add(rowDef);
                    mainGrid.Children.Add(item);
                    SetRow(item, index++);
                }

                _maxDate = _items.Where(x => x.MaxDateCurrent != null).Max(x => x.MaxDateCurrent);
                _minDate = _items.Where(x => x.MaxDateCurrent != null).Min(x => x.MinDateCurrent);

                BringScrollBarToZoom();

                Refresh();
            }
        }

        public void Refresh()
        {
            var minDate = _minDate.Value.AddSeconds(Scroll);
            var seconds = (_maxDate.Value - _minDate.Value).TotalSeconds / Zoom;
            var maxDate = minDate.AddSeconds(seconds);
            lblStart.Content = minDate.ToString();
            lblEnd.Content = maxDate.ToString();

            foreach (var item in _items)
            {
                item.MaxDate = _maxDate.Value;
                item.MinDate = _minDate.Value;

                item.Zoom = Zoom;
                item.Scroll = Scroll;

                item.SetColors(Brushes.SteelBlue, Brushes.Gray);

                item.Refresh();
            }
        }

        public double Zoom { get; set; } = 1;
        public int Scroll { get; set; } = 0;

        private void btMagnifyMinus_Click(object sender, RoutedEventArgs e)
        {
            Zoom++;
            BringScrollBarToZoom();
            Refresh();
        }

        private void btMagnifyAdd_Click(object sender, RoutedEventArgs e)
        {
            if (Zoom > 1)
            {
                Zoom--;
                BringScrollBarToZoom();
                Refresh();
            }
        }

        private void BringScrollBarToZoom()
        {
            _ingoreScrollEvent = true;
            var totalSeconds = (_maxDate.Value - _minDate.Value).TotalSeconds;
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
}
