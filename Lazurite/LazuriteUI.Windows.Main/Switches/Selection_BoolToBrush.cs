using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace LazuriteUI.Windows.Main.Switches
{
    [ValueConversion(typeof(bool), typeof(Brush))]
    public class Selection_BoolToBrush : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
                return Controls.Visual.SelectedSwitchBackground;
            else
                return Controls.Visual.SelectedSwitchBackgroundReadonly;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
