using PingPluginUtils;
using System.Windows;

namespace PingPluginUI
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IPingAction _pingAction;
        public MainWindow(IPingAction action)
        {
            InitializeComponent();
            _pingAction = action;
            btCancel.Click += (o, e) => this.DialogResult = false;
            btOk.Click += (o, e) =>
            {
                _pingAction.Host = tbHost.Text;
                this.DialogResult = true;
            };
        }
    }
}
