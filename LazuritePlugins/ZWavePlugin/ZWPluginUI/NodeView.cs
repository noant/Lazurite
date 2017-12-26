using LazuriteUI.Windows.Controls;
using OpenZWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZWPluginUI
{
    public class NodeView: ItemView
    {
        public NodeView(Node node)
        {
            Node = node;
            if (Node.Failed)
                this.Opacity = 0.8;
            this.Icon = LazuriteUI.Icons.Icon.Connect;
            if (node.ProductName.ToLower().Contains("usb"))
                this.Icon = LazuriteUI.Icons.Icon.Usb;
            if (node.ProductName.ToLower().Contains("light"))
                this.Icon = LazuriteUI.Icons.Icon.LightbulbHueOn;
            if (node.ProductName.ToLower().Contains("sensor"))
                this.Icon = LazuriteUI.Icons.Icon.ManSensor;

            this.Content = Node.ProductName;
            if (Node.Failed)
                this.Content = string.Format("id {0}; node failed;", Node.Id);
        }

        public Node Node { get; private set; }
    }
}
