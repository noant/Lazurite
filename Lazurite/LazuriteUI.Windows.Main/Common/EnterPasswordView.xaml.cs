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

namespace LazuriteUI.Windows.Main.Common
{
    /// <summary>
    /// Логика взаимодействия для EnterPasswordView.xaml
    /// </summary>
    public partial class EnterPasswordView : UserControl
    {
        public EnterPasswordView(string caption, Action<string> entered, Func<string, bool> validation = null, string notation="")
        {
            InitializeComponent();
            this.Loaded += (o, e) =>  FocusManager.SetFocusedElement(this, tbPassword);
            captionView.Content = caption;
            tbPassword.PasswordChanged += (o, e) => btApply.IsEnabled = validation?.Invoke(tbPassword.Password) ?? true;
            btApply.Click += (o, e) => entered?.Invoke(tbPassword.Password);
            labelNotation.Content = notation;
            btApply.IsEnabled = validation?.Invoke(string.Empty) ?? true;
        }

        public static void Show(string caption, Action<string> entered, Func<string, bool> validation = null, string notation="")
        {
            DialogView dialogView = null;
            var enterPasswordView = new EnterPasswordView(
                caption,
                (pass) =>
                {
                    dialogView.Close();
                    entered?.Invoke(pass);
                },
                validation,
                notation);
            dialogView = new DialogView(enterPasswordView);
            dialogView.Show();
        }
    }
}
