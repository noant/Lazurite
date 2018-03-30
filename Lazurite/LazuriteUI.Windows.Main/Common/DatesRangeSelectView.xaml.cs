using Lazurite.Shared;
using LazuriteUI.Windows.Controls;
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

namespace LazuriteUI.Windows.Main.Common
{
    /// <summary>
    /// Логика взаимодействия для DateTimeRangeSelectView.xaml
    /// </summary>
    public partial class DatesRangeSelectView : UserControl
    {
        public DatesRangeSelectView(DateTime max, DateTime min, DateSelectionItem selectionItem)
        {
            InitializeComponent();

            dt1.DisplayDateEnd = dt2.DisplayDateEnd = max;
            dt1.DisplayDateStart = dt2.DisplayDateStart = min;

            dt1.SelectedDate = selectionItem.Start;
            dt2.SelectedDate = selectionItem.End;
        }

        private void btRange_Click(object sender, RoutedEventArgs e)
        {
            Commited?.Invoke(this, new EventsArgs<DateSelectionItem>(new DateSelectionItem(dt1.SelectedDate ?? DateTime.Now.Date, dt2.SelectedDate ?? DateTime.Now.Date)));
        }

        private void btYear_Click(object sender, RoutedEventArgs e)
        {
            Commited?.Invoke(this, new EventsArgs<DateSelectionItem>(new DateSelectionItem(DateSelection.LastYear)));
        }

        private void btMonth_Click(object sender, RoutedEventArgs e)
        {
            Commited?.Invoke(this, new EventsArgs<DateSelectionItem>(new DateSelectionItem(DateSelection.LastMonth)));
        }

        private void btWeek_Click(object sender, RoutedEventArgs e)
        {
            Commited?.Invoke(this, new EventsArgs<DateSelectionItem>(new DateSelectionItem(DateSelection.LastWeek)));
        }

        private void btDay_Click(object sender, RoutedEventArgs e)
        {
            Commited?.Invoke(this, new EventsArgs<DateSelectionItem>(new DateSelectionItem(DateSelection.Last24Hours)));
        }

        public event EventsHandler<DateSelectionItem> Commited;

        public static void Show(Action<DateSelectionItem> selected, DateTime max, DateTime min, DateSelectionItem selectionItem)
        {
            var control = new DatesRangeSelectView(max, min, selectionItem);
            var dialog = new DialogView(control);
            control.Commited += (o, e) =>
            {
                selected?.Invoke(e.Value);
                dialog.Close();
            };
            dialog.Show();
        }
    }

    public enum DateSelection
    {
        LastYear,
        LastMonth,
        LastWeek,
        Last24Hours,
        Custom
    }

    public class DateSelectionItem
    {
        public DateSelectionItem(DateTime start, DateTime end)
        {
            DateSelection = DateSelection.Custom;
            Start = start;
            End = end;
        }

        public DateSelectionItem(DateSelection selection)
        {
            DateSelection = selection;
            switch (DateSelection)
            {
                case DateSelection.LastYear:
                    Start = DateTime.Now.AddYears(-1);
                    End = DateTime.Now;
                    break;

                case DateSelection.LastMonth:
                    Start = DateTime.Now.AddMonths(-1);
                    End = DateTime.Now;
                    break;

                case DateSelection.LastWeek:
                    Start = DateTime.Now.AddDays(-7);
                    End = DateTime.Now;
                    break;

                case DateSelection.Last24Hours:
                    Start = DateTime.Now;
                    End = DateTime.Now;
                    break;
            }
        }

        public DateSelection DateSelection { get; private set; }
        public DateTime Start { get; private set; }
        public DateTime End { get; private set; }
    }
}
