using PingPluginUtils;
using System.Windows;

namespace PingPluginUI
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(IPingAction action)
        {
            InitializeComponent();
            tbHost.Text = action.Host;
            btCancel.Click += (o, e) => DialogResult = false;
            btOk.Click += (o, e) =>
            {
                action.Host = tbHost.Text;
                DialogResult = true;
            };
        }
    }
}
