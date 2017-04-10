using PyriteUI.Controls;
using PyriteUI.Icons;
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

namespace PyriteUI.WpfTests
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        
        private void button_Copy1_Click(object sender, RoutedEventArgs e)
        {
            listItems.SelectionMode = Controls.ListViewItemsSelectionMode.Multiple;
        }

        private void button_Copy_Click(object sender, RoutedEventArgs e)
        {
            listItems.SelectionMode = Controls.ListViewItemsSelectionMode.Single;
        }

        private void button_Copy2_Click(object sender, RoutedEventArgs e)
        {
            listItems.SelectionMode = Controls.ListViewItemsSelectionMode.None;
        }

        private void ItemView_Click(object sender, RoutedEventArgs e)
        {
            ((ItemView)sender).Content = ((ItemView)sender).Selected.ToString();
        }
    }
}
