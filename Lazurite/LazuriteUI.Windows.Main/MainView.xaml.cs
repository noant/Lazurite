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

namespace LazuriteUI.Windows.Main
{
    /// <summary>
    /// Логика взаимодействия для MainView.xaml
    /// </summary>
    public partial class MainView : UserControl
    {
        public MainView()
        {
            InitializeComponent();
            this.Loaded += (o, e) => {
                this.menuItems.SelectionChanged += (o1, e1) => 
                    menuResolver.Resolver = menuItems.SelectedItem as IViewTypeResolverItem;
                this.menuItems.GetItems().First().Selected = true;
            };

            btHide.Click += (o, e) => {
                if (columnMenu.Width.Value == 170)
                {
                    columnMenu.Width = new GridLength(50);
                    rowTop.Height = new GridLength(0);
                    btHide.Icon = Icons.Icon.ArrowCollapsed;
                }
                else
                {
                    columnMenu.Width = new GridLength(170);
                    rowTop.Height = GridLength.Auto;
                    btHide.Icon = Icons.Icon.ArrowExpand;
                }
            };

            btRestart.Click += (o, e) => Utils.RestartService();
        }
    }
}
