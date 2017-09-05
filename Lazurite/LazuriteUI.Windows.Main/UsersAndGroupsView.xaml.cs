using LazuriteUI.Icons;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
