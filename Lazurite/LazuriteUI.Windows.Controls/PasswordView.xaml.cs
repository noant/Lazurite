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

namespace LazuriteUI.Windows.Controls
{
    /// <summary>
    /// Логика взаимодействия для PasswordView.xaml
    /// </summary>
    public partial class PasswordView : UserControl
    {
        public PasswordView()
        {
            InitializeComponent();
        }

        public string Password
        {
            get
            {
                return passwordBox.Password;
            }
            set
            {
                passwordBox.Password = value;
            }
        }

        public event RoutedEventHandler PasswordChanged
        {
            add
            {
                passwordBox.PasswordChanged += value;
            }
            remove
            {
                passwordBox.PasswordChanged -= value;
            }
        }
    }
}
