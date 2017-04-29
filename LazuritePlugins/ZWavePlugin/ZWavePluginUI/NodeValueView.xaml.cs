using OpenZWrapper;
using LazuriteUI.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ZWavePluginUI
{
    /// <summary>
    /// Логика взаимодействия для NodeValueView.xaml
    /// </summary>
    public partial class NodeValueView : UserControl, ISelectable
    {
        public NodeValueView(NodeValue nodeValue)
        {
            InitializeComponent();
            NodeValue = nodeValue;

            switch (NodeValue.ValueType)
            {
                case OpenZWrapper.ValueType.Bool:
                    itemView.Icon = LazuriteUI.Icons.Icon.TypeBoolean;
                    break;
                case OpenZWrapper.ValueType.Button:
                    itemView.Icon = LazuriteUI.Icons.Icon.InterfaceButton;
                    break;
                case OpenZWrapper.ValueType.Byte:
                case OpenZWrapper.ValueType.Int:
                case OpenZWrapper.ValueType.Short:
                case OpenZWrapper.ValueType.Decimal:
                    itemView.Icon = LazuriteUI.Icons.Icon.MeasureCentimeter;
                    break;
                case OpenZWrapper.ValueType.List:
                    itemView.Icon = LazuriteUI.Icons.Icon.InterfaceList;
                    break;
                case OpenZWrapper.ValueType.String:
                    itemView.Icon = LazuriteUI.Icons.Icon.PageText;
                    break;
            }

            this.itemView.SelectionChanged += (o, e) => {
                SelectionChanged?.Invoke(this, e);
            };
        }

        private NodeValue _nodeValue;
        public NodeValue NodeValue
        {
            get
            {
                return _nodeValue;
            }
            private set
            {
                _nodeValue = value;
                this.itemView.Content = _nodeValue.Name;
            }
        }

        public bool Selected
        {
            get
            {
                return itemView.Selected;
            }
            set
            {
                itemView.Selected = value;
            }
        }

        public bool Selectable
        {
            get
            {
                return itemView.Selectable;
            }
            set
            {
                itemView.Selectable = value;
            }
        }

        public event RoutedEventHandler Click
        {
            add
            {
                itemView.Click += value;
            }
            remove
            {
                itemView.Click -= value;
            }
        }

        public event RoutedEventHandler SelectionChanged;
    }
}
