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
            Content = string.Format("{0} ({1}) (id {2})", node?.ProductName ?? "[название загружается]", controller.Path, controller.HomeID);
            if (controller.IsHID)
                Icon = LazuriteUI.Icons.Icon.UsbDrive;
            else
                Icon = LazuriteUI.Icons.Icon.ChevronRight;
            if (controller.Failed)
            {
                Content = Content.ToString() + " (?)";
                ToolTip = "Возможны неполадки";
            }
        }

        public Controller Controller { get; private set; }
    }
}
