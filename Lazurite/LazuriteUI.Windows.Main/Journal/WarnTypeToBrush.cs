using Lazurite.Windows.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Media;

namespace LazuriteUI.Windows.Main.Journal
{
    [ValueConversion(typeof(WarnType), typeof(Brush))]
    public class WarnTypeToBrush : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((WarnType)value) {
                case WarnType.Debug:
                    return new SolidColorBrush(Colors.LightGray);
                case WarnType.Error:
                    return new SolidColorBrush(Colors.OrangeRed);
                case WarnType.Fatal:
                    return new SolidColorBrush(Colors.Crimson);
                case WarnType.Info:
                    return new SolidColorBrush(Colors.SteelBlue);
                case WarnType.Warn:
                    return new SolidColorBrush(Colors.Yellow);
            };

            throw new Exception("unknown warning type");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
