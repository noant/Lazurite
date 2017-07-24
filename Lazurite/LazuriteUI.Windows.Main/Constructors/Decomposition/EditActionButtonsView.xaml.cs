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
    public partial class EditActionButtonsView : Grid
    {
        public static readonly DependencyProperty EditVisibleProperty = DependencyProperty.Register(nameof(EditVisible), typeof(bool), typeof(EditActionButtonsView), new FrameworkPropertyMetadata(true)
        {
            PropertyChangedCallback = (o, e) =>
            {
                ((EditActionButtonsView)o).btEdit.Visibility = (bool)e.NewValue ? Visibility.Visible : Visibility.Collapsed;
            }
        });
        public static readonly DependencyProperty ChangeVisibleProperty = DependencyProperty.Register(nameof(ChangeVisible), typeof(bool), typeof(EditActionButtonsView), new FrameworkPropertyMetadata(true)
        {
            PropertyChangedCallback = (o, e) =>
            {
                ((EditActionButtonsView)o).btChange.Visibility = (bool)e.NewValue ? Visibility.Visible : Visibility.Collapsed;
            }
        });

        public EditActionButtonsView()
        {
            InitializeComponent();

            btChange.Click += (o, e) => ChangeClick?.Invoke();
            btEdit.Click += (o, e) => EditClick?.Invoke();
        }
        
        public bool EditVisible
        {
            get
            {
                return (bool)GetValue(EditVisibleProperty);
            }
            set
            {
                SetValue(EditVisibleProperty, value);
            }
        }

        public bool ChangeVisible
        {
            get
            {
                return (bool)GetValue(ChangeVisibleProperty);
            }
            set
            {
                SetValue(ChangeVisibleProperty, value);
            }
        }

        public event Action EditClick;
        public event Action ChangeClick;
    }
}
