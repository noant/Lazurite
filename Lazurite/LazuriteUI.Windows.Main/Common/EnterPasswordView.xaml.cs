using LazuriteUI.Windows.Controls;
using System;
using System.Windows.Controls;
using System.Windows.Input;

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
            Loaded += (o, e) =>  FocusManager.SetFocusedElement(this, tbPassword);
            captionView.Content = caption;
            tbPassword.PasswordChanged += (o, e) => btApply.IsEnabled = (validation?.Invoke(tbPassword.Password) ?? true) && LocalValidation();
            tbPasswordRepeat.PasswordChanged += (o, e) => btApply.IsEnabled = (validation?.Invoke(tbPassword.Password) ?? true) && LocalValidation();
            btApply.Click += (o, e) => entered?.Invoke(tbPassword.Password);
            labelNotation.Content = notation;
            btApply.IsEnabled = validation?.Invoke(string.Empty) ?? true;
        }

        private bool LocalValidation()
        {
            if (tbPassword.Password != tbPasswordRepeat.Password)
            {
                labelError.Content = "Повторный ввод неверен.";
                return false;
            }
            else
            {
                labelError.Content = string.Empty;
                return true;
            }
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
