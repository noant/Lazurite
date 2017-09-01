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

namespace LazuriteUI.Windows.Launcher
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class RequireAdminRightsWindow : Window
    {
        public RequireAdminRightsWindow()
        {
            InitializeComponent();

            btApply.Click += (o, e) => ApplyClick?.Invoke();            
            btCancel.Click += (o, e) => CancelClick?.Invoke();
        }

        public event Action ApplyClick;
        public event Action CancelClick;
    }
}
