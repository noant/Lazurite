using LazuriteUI.Icons;
using System.ComponentModel;
using System.Windows.Controls;

namespace LazuriteUI.Windows.Main
{
    /// <summary>
    /// Логика взаимодействия для UsersAndGroupsView.xaml
    /// </summary>
    [DisplayName("Пользователи и группы")]
    [LazuriteIcon(Icon.Group)]
    public partial class UsersAndGroupsView : Grid
    {
        public UsersAndGroupsView()
        {
            InitializeComponent();
        }
    }
}
