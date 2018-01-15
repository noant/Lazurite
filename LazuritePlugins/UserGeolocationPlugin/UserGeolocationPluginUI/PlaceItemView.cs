using LazuriteUI.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserGeolocationPlugin;

namespace UserGeolocationPluginUI
{
    public class PlaceItemView: ItemView
    {
        public GeolocationPlace Place { get; private set; }

        public PlaceItemView(GeolocationPlace place)
        {
            Place = place;
            this.Icon = LazuriteUI.Icons.Icon.ChevronRight;
            this.Content = place.Name;
            this.Margin = new System.Windows.Thickness(0, 1, 0, 0);
        }
    }
}
