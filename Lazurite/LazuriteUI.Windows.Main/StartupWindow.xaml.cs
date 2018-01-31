using System.Threading;
using System.Windows;

namespace LazuriteUI.Windows.Main
{
    /// <summary>
    /// Логика взаимодействия для SplashView.xaml
    /// </summary>
    public partial class StartupWindow : Window
    {
        public StartupWindow()
        {
            InitializeComponent();
            this.progress.StartProgress();
        }
    }
}
