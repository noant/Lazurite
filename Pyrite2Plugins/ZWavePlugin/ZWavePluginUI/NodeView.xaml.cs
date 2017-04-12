using OpenZWrapper;
using PyriteUI.Controls;
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
    /// Логика взаимодействия для NodeView.xaml
    /// </summary>
    public partial class NodeView : UserControl, ISelectable
    {
        public NodeView(Node node)
        {
            InitializeComponent();
            this.itemView.SelectionChanged += (o, e) => {
                SelectionChanged?.Invoke(this, e);
            };
            Node = node;

            // =)
            this.itemView.Icon = PyriteUI.Icons.Icon.Connect;
            if (node.ProductName.ToLower().Contains("usb"))
                this.itemView.Icon = PyriteUI.Icons.Icon.Usb;
            if (node.ProductName.ToLower().Contains("light"))
                this.itemView.Icon = PyriteUI.Icons.Icon.LightbulbHueOn;
            if (node.ProductName.ToLower().Contains("sensor"))
                this.itemView.Icon = PyriteUI.Icons.Icon.ManSensor;
        }
        
        private Node _node;

        public event RoutedEventHandler SelectionChanged;
        
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
        
        public Node Node
        {
            get
            {
                return _node;
            }
            private set
            {
                _node = value;
                this.itemView.Content = _node.ProductName;
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
    }
}
