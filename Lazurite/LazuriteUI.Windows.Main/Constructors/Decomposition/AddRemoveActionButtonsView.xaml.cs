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

namespace LazuriteUI.Windows.Main.Constructors.Decomposition
{
    /// <summary>
    /// Логика взаимодействия для EditActionButtonsView.xaml
    /// </summary>
    public partial class AddRemoveActionButtonsView : Grid
    {
        public static readonly DependencyProperty RemoveVisibleProperty = DependencyProperty.Register(nameof(RemoveVisible), typeof(bool), typeof(AddRemoveActionButtonsView), new FrameworkPropertyMetadata(true)
        {
            PropertyChangedCallback = (o, e) =>
            {
                ((AddRemoveActionButtonsView)o).btRemove.Visibility = (bool)e.NewValue ? Visibility.Visible : Visibility.Collapsed;
            }
        });
        public static readonly DependencyProperty AddVisibleProperty = DependencyProperty.Register(nameof(AddVisible), typeof(bool), typeof(AddRemoveActionButtonsView), new FrameworkPropertyMetadata(true)
        {
            PropertyChangedCallback = (o, e) =>
            {
                ((AddRemoveActionButtonsView)o).btAddNew.Visibility = (bool)e.NewValue ? Visibility.Visible : Visibility.Collapsed;
            }
        });

        public AddRemoveActionButtonsView()
        {
            InitializeComponent();

            btAddNew.Click += (o, e) => AddNewClick?.Invoke();
            btRemove.Click += (o, e) => RemoveClick?.Invoke();
        }

        public bool RemoveVisible
        {
            get
            {
                return (bool)GetValue(RemoveVisibleProperty);
            }
            set
            {
                SetValue(RemoveVisibleProperty, value);
            }
        }

        public bool AddVisible
        {
            get
            {
                return (bool)GetValue(AddVisibleProperty);
            }
            set
            {
                SetValue(AddVisibleProperty, value);
            }
        }
        
        public event Action RemoveClick;
        public event Action AddNewClick;
    }
}
