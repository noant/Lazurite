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
        }

        public DateTime DateTime
        {
            get
            {
                var selectedDate = datePicker.SelectedDate ?? new DateTime();
                return new DateTime(selectedDate.Year, selectedDate.Month, selectedDate.Day, (int)nudHour.Value, (int)nudMinute.Value, (int)nudSecond.Value);
            }
            set
            {
                datePicker.SelectedDate = value;
                nudHour.Value = value.Hour;
                nudMinute.Value = value.Minute;
                nudSecond.Value = value.Second;
            }
        }

        public event Action<object, RoutedEventArgs> Apply;
    }
}
