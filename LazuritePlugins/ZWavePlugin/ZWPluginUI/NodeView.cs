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
            if (node.ProductName?.ToLower().Contains("usb") ?? false)
                this.Icon = LazuriteUI.Icons.Icon.Usb;
            if (node.ProductName?.ToLower().Contains("light") ?? false)
                this.Icon = LazuriteUI.Icons.Icon.LightbulbHueOn;
            if (node.ProductName?.ToLower().Contains("sensor") ?? false)
                this.Icon = LazuriteUI.Icons.Icon.ManSensor;    
            if (Node.Failed)
                Opacity = 0.5;
            var caption = Node.FullName;
            caption = caption.Length > 55 ? caption.Substring(0, 52) + "..." : caption;
            Content = caption;
            Margin = new System.Windows.Thickness(1);
        }

        public Node Node { get; private set; }
    }
}
