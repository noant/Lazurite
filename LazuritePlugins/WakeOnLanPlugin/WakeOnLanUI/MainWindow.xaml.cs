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
using WakeOnLanUtils;

namespace WakeOnLanUI
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(IWakeOnLanAction action)
        {
            InitializeComponent();
            actionView.Apply += () => DialogResult = true;
            actionView.Cancel += () => DialogResult = false;
            actionView.RefreshWith(action);
        }
    }
}
