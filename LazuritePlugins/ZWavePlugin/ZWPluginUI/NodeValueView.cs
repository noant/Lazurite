using LazuriteUI.Windows.Controls;
using OpenZWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ZWPluginUI
{
    public class NodeValueView: ItemView
    {
        public NodeValueView(NodeValue value, Func<NodeValue,bool> isComparable)
        {
            NodeValue = value;
            switch (NodeValue.ValueType)
            {
                case OpenZWrapper.ValueType.Bool:
                    Icon = LazuriteUI.Icons.Icon.TypeBoolean;
                    break;
                case OpenZWrapper.ValueType.Button:
                    Icon = LazuriteUI.Icons.Icon.InterfaceButton;
                    break;
                case OpenZWrapper.ValueType.Byte:
                case OpenZWrapper.ValueType.Int:
                case OpenZWrapper.ValueType.Short:
                case OpenZWrapper.ValueType.Decimal:
                    Icon = LazuriteUI.Icons.Icon.MeasureCentimeter;
                    break;
                case OpenZWrapper.ValueType.List:
                    Icon = LazuriteUI.Icons.Icon.InterfaceList;
                    break;
                case OpenZWrapper.ValueType.String:
                    Icon = LazuriteUI.Icons.Icon.PageText;
                    break;
            }

            switch (NodeValue.Genre)
            {
                case ValueGenre.Basic:
                    Background = new SolidColorBrush(Color.FromRgb(25, 35 ,25));
                    break;
                case ValueGenre.User:
                    Background = new SolidColorBrush(Color.FromRgb(25, 25, 35));
                    break;
                case ValueGenre.Config:
                    Background = new SolidColorBrush(Color.FromRgb(35, 25, 35));
                    break;
            }

            this.IsEnabled = isComparable?.Invoke(NodeValue) ?? true;
            this.Content = NodeValue.Name.Length > 40 ? NodeValue.Name.Substring(0, 37) + "..." : NodeValue.Name;
            this.ToolTip = string.Format("{0} (ID={1})", NodeValue.Name, NodeValue.Id);
            this.Margin = new System.Windows.Thickness(1);
        }

        public NodeValue NodeValue { get; private set; }
    }
}
