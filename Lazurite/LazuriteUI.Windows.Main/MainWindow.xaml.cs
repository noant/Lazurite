using Lazurite.IOC;
using Lazurite.Shared;
using Lazurite.Utils;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace LazuriteUI.Windows.Main
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IHardwareVolumeChanger
    {
        public MainWindow()
        {
            InitializeComponent();
            Icon = BitmapFrame.Create(Icons.Utils.GetIconData(Icons.Icon.Lazurite64));

            Singleton.Clear<IHardwareVolumeChanger>();
            Singleton.Add(this);

            MouseWheel += Window_MouseWheel;
        }

        public event EventsHandler<int> VolumeDown;
        public event EventsHandler<int> VolumeUp;

        private void Window_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta < 0)
                VolumeDown?.Invoke(this, new EventsArgs<int>(-1));
            else
                VolumeUp?.Invoke(this, new EventsArgs<int>(1));
        }
    }
}
