using Lazurite.Shared;
using LazuriteUI.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserGeolocationPluginUI
{
    public class DevicesView : ListItemsView
    {
        public DevicesView()
        {
            this.SelectionMode = ListViewItemsSelectionMode.Single;
        }

        public string[] Devices
        {
            get
            {
                return this.GetItems().Cast<DeviceItemView>().Select(x => x.Device).ToArray();
            }
            set
            {
                this.Children.Clear();
                foreach (var device in value)
                    this.Children.Add(new DeviceItemView(device));
            }
        }

        public string SelectedDevice
        {
            get
            {
                return (this.SelectedItem as DeviceItemView)?.Device;
            }
            set
            {
                this.GetItems()
                    .Cast<DeviceItemView>()
                    .Where(x => x.Device.Equals(value))
                    .All(x => x.Selected = true);
            }
        }
    }
}
