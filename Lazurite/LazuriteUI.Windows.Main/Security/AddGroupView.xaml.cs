using Lazurite.MainDomain;
using LazuriteUI.Windows.Controls;
using System;
using System.Windows.Controls;

namespace LazuriteUI.Windows.Main.Security
{
    /// <summary>
    /// Логика взаимодействия для AddUserView.xaml
    /// </summary>
    public partial class AddGroupView : UserControl
    {
        public AddGroupView(UserGroupBase group)
        {
            InitializeComponent();
            
            tbName.TextChanged += (o, e) => Validate();

            btApply.Click += (o, e) => {
                group.Name = tbName.Text.Trim();
                OkClicked?.Invoke();
            };
            
            tbName.Text = group.Name;

            Validate();
        }

        public Action<GroupValidationArgs> Validation { get; set; }

        public void Validate()
        {
            var args = new GroupValidationArgs(tbName.Text.Trim());
            Validation?.Invoke(args);
            if (string.IsNullOrEmpty(args.Name))
            {
                args.Success = false;
                args.Message = "Необходимо ввести наименование группы";
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

        public static void Show(Action callback, Action<GroupValidationArgs> validation, UserGroupBase group) {
            var control = new AddGroupView(group);
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

    public class GroupValidationArgs
    {
        public GroupValidationArgs(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }

        public bool Success { get; set; } = false;
        public string Message { get; set; }
    }
}
