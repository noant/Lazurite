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
        public static DependencyProperty RemoveVisibleProperty = DependencyProperty.Register(nameof(RemoveVisible), typeof(bool), typeof(EditActionButtonsView), new PropertyMetadata(true)); 
        public static DependencyProperty AddVisibleProperty = DependencyProperty.Register(nameof(AddVisible), typeof(bool), typeof(EditActionButtonsView), new PropertyMetadata(true)); 
        public static DependencyProperty EditVisibleProperty = DependencyProperty.Register(nameof(EditVisible), typeof(bool), typeof(EditActionButtonsView), new PropertyMetadata(true)); 
        public static DependencyProperty ChangeVisibleProperty = DependencyProperty.Register(nameof(ChangeVisible), typeof(bool), typeof(EditActionButtonsView), new PropertyMetadata(true)); 
        
        public EditActionButtonsView()
        {
            InitializeComponent();

            btAddNew.Click += (o, e) => AddNewClick?.Invoke();
            btChange.Click += (o, e) => ChangeClick?.Invoke();
            btEdit.Click += (o, e) => EditClick?.Invoke();
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
        public event Action RemoveClick;
        public event Action AddNewClick;
    }
}
