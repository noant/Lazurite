using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace LazuriteUI.Windows.Controls
{
    public class EntryView: TextBox
    {
        public EntryView()
        {
            this.Background = new SolidColorBrush(Colors.Transparent);
            this.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;
            this.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left;
            this.Foreground = new SolidColorBrush(Colors.White);
            this.BorderThickness = new System.Windows.Thickness(0, 0, 0, 2);
            this.BorderBrush = new SolidColorBrush(Colors.SteelBlue);
        }
    }
}
