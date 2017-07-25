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
    public partial class EditUserView : UserControl
    {
        public EditUserView(UserBase user)
        {
            InitializeComponent();

            tbLogin.Validation = (str) => str.Replace(" ", "");
            tbLogin.TextChanged += (o, e) => Validate();
            tbName.TextChanged += (o, e) => Validate();

            btApply.Click += (o, e) => {
                user.Name = tbName.Text.Trim();
                user.Login = tbLogin.Text;
                OkClicked?.Invoke();
            };

            tbLogin.Text = user.Login;
            tbName.Text = user.Name;

            Validate();
        }

        public Action<UserParamsValidationArgs> Validation { get; set; }

        public void Validate()
        {
            var args = new UserParamsValidationArgs(tbLogin.Text, tbName.Text.Trim());
            Validation?.Invoke(args);
            if (string.IsNullOrEmpty(args.Login))
            {
                args.Success = false;
                args.Message = "Необходимо ввести логин пользователя";
            }
            else if (string.IsNullOrEmpty(args.Name))
            {
                args.Success = false;
                args.Message = "Необходимо ввести имя пользователя";
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

        public static void Show(Action callback, Action<UserParamsValidationArgs> validation, UserBase user) {
            var control = new EditUserView(user);
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

    public class UserParamsValidationArgs
    {
        public UserParamsValidationArgs(string login, string name)
        {
            Name = name;
            Login = login;
        }

        public string Name { get; private set; }
        public string Login { get; private set; }

        public bool Success { get; set; } = false;
        public string Message { get; set; }
    }
}
