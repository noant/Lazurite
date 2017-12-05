using System.Windows;
using System.Windows.Controls;

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
