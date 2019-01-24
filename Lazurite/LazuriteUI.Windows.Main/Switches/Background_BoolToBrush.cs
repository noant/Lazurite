using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace LazuriteUI.Windows.Main.Switches
{
    [ValueConversion(typeof(bool), typeof(Brush))]
    public class Background_BoolToBrush : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
                return Controls.Visual.SwitchBackground;
            else
                return Controls.Visual.SwitchBackgroundReadonly;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
