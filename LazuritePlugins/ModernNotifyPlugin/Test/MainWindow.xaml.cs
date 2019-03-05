using Lazurite.ActionsDomain;
using ModernNotifyPlugin;
using NotificationUI;
using NotificationUITV;
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

namespace Test
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Notify _notify = new Notify();

        private NotificationWindowTv _tv = new NotificationWindowTv();

        public MainWindow()
        {
            InitializeComponent();
            _tv.Show();
            //_notify.Initialize();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var color = Colors.Red;
            var sec = DateTime.Now.Second;
            if (sec % 2 == 0)
                color = Colors.Blue;
            else if (sec % 3 == 0)
                color = Colors.Black;
            else if (sec % 5 == 0)
                color = Colors.BlueViolet;
            _tv.Notify(tb.Text, false, new SolidColorBrush(color));
            //_notify.SetValue(null, tb.Text);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //NotificationWindow.SoundNotify();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            //NotificationWindow.SoundNotifyAccept();

        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            //NotificationWindow.SoundNotifyDisallow();

        }
    }
}
