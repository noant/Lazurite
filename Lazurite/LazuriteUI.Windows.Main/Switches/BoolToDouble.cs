﻿using LazuriteUI.Icons;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace LazuriteUI.Windows.Main.Switches
{
    [ValueConversion(typeof(bool), typeof(double))]
    public class BoolToDouble : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
                return 1.0d;
            else
                return 0.35d;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
