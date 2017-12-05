using LazuriteUI.Windows.Controls;
using OpenZWrapper;
using System.Windows;
using System.Windows.Controls;

namespace ZWavePluginUI
{
    /// <summary>
    /// Логика взаимодействия для ControllerView.xaml
    /// </summary>
    public partial class ControllerView : UserControl, ISelectable
    {
        public ControllerView(Controller controller, ZWaveManager manager)
        {
            InitializeComponent();
            Controller = controller;
            var node = manager.GetControllerNode(controller);
            this.itemView.Content = string.Format("{0} ({1})", node.ProductName, controller.Path);
        }

        public Controller Controller { get; private set; }

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

        public event RoutedEventHandler SelectionChanged
        {
            add
            {
                itemView.SelectionChanged += value;
            }
            remove
            {
                itemView.SelectionChanged -= value;
            }
        }
    }
}
