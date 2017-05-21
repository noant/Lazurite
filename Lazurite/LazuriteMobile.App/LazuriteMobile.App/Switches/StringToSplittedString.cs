using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace LazuriteMobile.App.Switches
{
    public class StringToSplittedString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var str = value.ToString();
            if (str.Length < 15)
                return str;
            if (str.Length > 14)
            {
                var min = int.MaxValue;
                for (int i = 0; i<str.Length; i++)
                {
                    if (str[i].Equals(' '))
                        if (Math.Abs(str.Length / 2 - i) < Math.Abs(min))
                            min = Math.Abs(str.Length / 2 - i);
                }
                if (min != int.MaxValue)
                    str = str.Insert(str.Length / 2 + min, "\r\n").Replace(" \r\n", "\r\n");
            }
            return str;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
