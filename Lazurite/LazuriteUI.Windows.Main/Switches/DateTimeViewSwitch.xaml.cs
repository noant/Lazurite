using LazuriteUI.Windows.Controls;
using System;
using System.Windows;
using System.Windows.Controls;

namespace LazuriteUI.Windows.Main.Switches
{
    /// <summary>
    /// Логика взаимодействия для DateTimeViewSwitch.xaml
    /// </summary>
    public partial class DateTimeViewSwitch : UserControl
    {
        public DateTimeViewSwitch()
        {
            InitializeComponent();
            itemViewApply.Click += (o, e) => Apply?.Invoke(this, new RoutedEventArgs());
            tbHour.Validation = (v) => EntryViewValidation.IntValidation("Час", 0, 23).Invoke(v);
            tbMinute.Validation = (v) => EntryViewValidation.IntValidation("Минута", 0, 59).Invoke(v);
            tbSecond.Validation = (v) => EntryViewValidation.IntValidation("Секунда", 0, 59).Invoke(v);
        }

        public DateTime DateTime
        {
            get
            {
                var selectedDate = datePicker.SelectedDate ?? new DateTime();
                return new DateTime(selectedDate.Year, selectedDate.Month, selectedDate.Day, int.Parse(tbHour.Text), int.Parse(tbMinute.Text), int.Parse(tbSecond.Text));
            }
            set
            {
                datePicker.SelectedDate = value;
                tbHour.Text = value.Hour.ToString();
                tbMinute.Text = value.Minute.ToString();
                tbSecond.Text = value.Second.ToString();
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event Action<object, RoutedEventArgs> Apply;
    }
}
