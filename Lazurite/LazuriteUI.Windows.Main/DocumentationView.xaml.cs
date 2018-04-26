using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Логика взаимодействия для DocumentationView.xaml
    /// </summary>
    public partial class DocumentationView : Grid
    {
        public DocumentationView()
        {
            InitializeComponent();
        }

        private void ItemView_Click(object sender, RoutedEventArgs e)
        {
            var url = "https://github.com/noant/Lazurite/wiki";

            try
            {
                Process.Start(url);
            }
            catch
            {
                Process.Start("IEXPLORE.EXE", url); //crutch
            }
        }
    }
}
