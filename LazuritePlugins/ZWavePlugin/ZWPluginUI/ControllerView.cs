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
            var name = string.Format("{0} ({1}) (HomeID={2})", node?.FullName ?? "[название загружается]", controller.Path, controller.HomeID);
            ToolTip = name;
            if (name.Length > 40)
                name = name.Substring(0, 37)+"...";
            Content = name;

            if (controller.IsHID)
                Icon = LazuriteUI.Icons.Icon.UsbDrive;
            else
                Icon = LazuriteUI.Icons.Icon.ChevronRight;

            if (controller.Failed)
            {
                Opacity = 0.5;
                ToolTip += " Возможны неполадки или устройство еще не проинициализировано";
            }
        }

        public Controller Controller { get; private set; }
    }
}
