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
        }

        private IDiagramItem[] _items;

        public void SetItems(IDiagramItem[] items)
        {
            _items = items;
            mainGrid.Children.Clear();
            mainGrid.RowDefinitions.Clear();

            var index = 0;
            foreach(FrameworkElement item in items)
            {
                var rowDef = new RowDefinition();
                rowDef.Height = new GridLength(((IDiagramItem)item).RequireLarge ? 2 : 1, GridUnitType.Star);
                mainGrid.RowDefinitions.Add(rowDef);
                mainGrid.Children.Add(item);
                SetRow(item, index++);
            }

            Refresh();
        }

        public void Refresh()
        {
            var maxDate = _items.Where(x=>x.MaxDateCurrent != null).Max(x => x.MaxDateCurrent);
            var minDate = _items.Where(x => x.MaxDateCurrent != null).Min(x => x.MinDateCurrent);

            foreach (var item in _items)
            {
                item.MaxDate = maxDate.Value;
                item.MinDate = minDate.Value;

                item.Zoom = Zoom;
                item.Scroll = Scroll;

                item.SetColors(Brushes.SteelBlue, Brushes.Gray);

                item.Refresh();
            }
        }

        public double Zoom { get; set; } = 1;
        public double Scroll { get; set; } = 0;
    }
}
