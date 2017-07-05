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
            tbHour.Validation = (str) => int.Parse(str) >= 0 && int.Parse(str) <= 23;
            tbMinute.Validation = (str) => int.Parse(str) >= 0 && int.Parse(str) <= 59;
            tbSecond.Validation = (str) => int.Parse(str) >= 0 && int.Parse(str) <= 59;
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

        public event Action<object, RoutedEventArgs> Apply;
    }
}
