using System;

using Xamarin.Forms;

namespace LazuriteMobile.App.Switches
{
    public partial class DateTimeViewSwitch : ContentView
    {
        public DateTimeViewSwitch()
        {
            InitializeComponent();
        }

        private void itemViewApply_Click(object arg1, EventArgs arg2)
        {
            Apply?.Invoke(this, new EventArgs());
        }

        public DateTime DateTime
        {
            get
            {
                var selectedDate = datePicker.Date;
                return new DateTime(selectedDate.Year, selectedDate.Month, selectedDate.Day, (int)neHour.Value, (int)neMinute.Value, (int)neSecond.Value);
            }
            set
            {
                datePicker.Date = value;
                neHour.Value = value.Hour;
                neMinute.Value = value.Minute;
                neSecond.Value = value.Second;
            }
        }

        public event Action<object, EventArgs> Apply;
    }
}
