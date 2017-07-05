using LazuriteUI.Windows.Controls;
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

namespace LazuriteUI.Windows.Main.Constructors.StandardActionsInitialization
{
    /// <summary>
    /// Логика взаимодействия для StatusInitializationViewItem.xaml
    /// </summary>
    public partial class StatusInitializationViewItem : UserControl, ISelectable
    {
        public StatusInitializationViewItem()
        {
            InitializeComponent();
            this.itemView.SelectionChanged += (o, e) => SelectionChanged?.Invoke(this, e);
            this.itemViewRemove.Click += (o, e) => RemoveClick(this);
        }

        public bool IsRemoveButtonVisible
        {
            get
            {
                return itemViewRemove.Visibility == Visibility.Visible;
            }
            set
            {
                itemViewRemove.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
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

        public string Text
        {
            get
            {
                return itemView.Content.ToString();
            }
            set
            {
                itemView.Content = value;
            }
        }

        public event RoutedEventHandler SelectionChanged;

        public event Action<StatusInitializationViewItem> RemoveClick;
    }
}
