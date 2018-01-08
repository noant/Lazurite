using LazuriteUI.Windows.Controls;
using OpenZWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZWPluginUI
{
    public class ControllerView: ItemView
    {
        public ControllerView(Controller controller, ZWaveManager manager)
        {
            Controller = controller;
            var node = manager.GetControllerNode(controller);
            this.Content = string.Format("{0} ({1})", node?.ProductName ?? "[название загружается]", controller.Path);
            if (controller.IsHID)
                this.Icon = LazuriteUI.Icons.Icon.UsbDrive;
            else
                this.Icon = LazuriteUI.Icons.Icon.ChevronRight;
            this.IsEnabled = !controller.Failed;
        }

        public Controller Controller { get; private set; }
    }
}
