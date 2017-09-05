using Lazurite.MainDomain;
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

namespace LazuriteUI.Windows.Main.Security
{
    /// <summary>
    /// Логика взаимодействия для AddUserView.xaml
    /// </summary>
    public partial class EditUserPasswordView : UserControl
    {
        public EditUserPasswordView(UserBase user)
        {
            InitializeComponent();
            
            tbPassword.PasswordChanged += (o, e) => Validate();
            tbPasswordRepeat.PasswordChanged+= (o, e) => Validate();

            btApply.Click += (o, e) => {
                user.PasswordHash = Lazurite.Windows.Utils.CryptoUtils.CreatePasswordHash(tbPassword.Password);
                OkClicked?.Invoke();
            };
            
            Validate();
        }

        public Action<UserPasswordValidationArgs> Validation { get; set; }

        public void Validate()
        {
            var args = new UserPasswordValidationArgs(tbPassword.Password, tbPasswordRepeat.Password);
            Validation?.Invoke(args);
            if (string.IsNullOrEmpty(args.Password))
            {
                args.Success = false;
                args.Message = "Необходимо ввести новый пароль пользователя";
            }
            else if (string.IsNullOrEmpty(args.PasswordRepeat))
            {
                args.Success = false;
                args.Message = "Необходимо повторить ввод пароля";
            }
            else if (!args.Password.Equals(args.PasswordRepeat))
            {
                args.Success = false;
                args.Message = "Пароли не совпадают";
            }
            if (args.Success)
            {
                tbValidation.Text = string.Empty;
                btApply.IsEnabled = true;
            }
            else
            {
                tbValidation.Text = args.Message;
                btApply.IsEnabled = false;
            }
        }

        public event Action OkClicked;

        public static void Show(Action callback, Action<UserPasswordValidationArgs> validation, UserBase user) {
            var control = new EditUserPasswordView(user);
            var dialog = new DialogView(control);
            control.Validation = validation;
            control.OkClicked += () =>
            {
                callback?.Invoke();
                dialog.Close();
            };
            dialog.Show();
        }
    }

    public class UserPasswordValidationArgs
    {
        public UserPasswordValidationArgs(string password, string passwordRepeat)
        {
            Password = password;
            PasswordRepeat = passwordRepeat;
        }

        public string Password { get; private set; }
        public string PasswordRepeat { get; private set; }

        public bool Success { get; set; } = false;
        public string Message { get; set; }
    }
}
