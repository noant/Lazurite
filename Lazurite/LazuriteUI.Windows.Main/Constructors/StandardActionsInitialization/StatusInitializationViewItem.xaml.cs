using Lazurite.Shared;
using LazuriteUI.Windows.Controls;
using System.Windows;
using System.Windows.Controls;

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
            itemView.SelectionChanged += (o, e) => SelectionChanged?.Invoke(this, e);
            itemViewRemove.Click += (o, e) => RemoveClick(this, new EventsArgs<StatusInitializationViewItem>(this));
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event EventsHandler<StatusInitializationViewItem> RemoveClick;
    }
}
