using System.Windows;
using WakeOnLanPlugin;

namespace Test
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += (o, e) => {
                var action = new WakeOnLanAction();
                action.UserInitializeWith(null, false);
                var act = action;
            };
        }
    }
}
