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
            this.SelectionChanged += (o, e) =>
            {
                SelectedDeviceChanged?.Invoke(this, new EventsArgs<string>(this.SelectedDevice));
            };
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
                return (this.GetSelectedItems().FirstOrDefault() as DeviceItemView)?.Device;
            }
            set
            {
                this.GetItems()
                    .Cast<DeviceItemView>()
                    .Where(x => x.Device.Equals(value))
                    .All(x => x.Selected = true);
                if (value == null)
                    SelectedDeviceChanged?.Invoke(this, new EventsArgs<string>(null));
            }
        }

        public event EventsHandler<string> SelectedDeviceChanged;
    }
}
