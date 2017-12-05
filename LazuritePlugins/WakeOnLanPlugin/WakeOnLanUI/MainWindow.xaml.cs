using System.Windows;
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
