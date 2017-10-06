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
using System.Windows.Shapes;

namespace LazuriteUI.Windows.Main
{
    /// <summary>
    /// Логика взаимодействия для FastSwitchWindow.xaml
    /// </summary>
    public partial class FastSwitchWindow : Window
    {
        public FastSwitchWindow()
        {
            InitializeComponent();
            //this.Loaded += (o, e) => switchesGrid.Initialize();
            switchesGrid.Initialize();
        }

        private void gridBack_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }
    }
}
