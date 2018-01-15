using LazuriteUI.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace UserGeolocationPluginUI
{
    public class DeviceItemView: ItemView
    {
        public string Device { get; private set; }

        public DeviceItemView(string device)
        {
            this.Icon = LazuriteUI.Icons.Icon.ChevronRight;
            this.Content = device;
            this.Device = device;
            this.Margin = new System.Windows.Thickness(0, 1, 0, 0);
            this.Background = Brushes.DarkSlateBlue;
        }
    }
}
