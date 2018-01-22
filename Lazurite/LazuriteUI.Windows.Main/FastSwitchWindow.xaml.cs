using Lazurite.IOC;
using Lazurite.Shared;
using Lazurite.Utils;
using System.Windows;
using System.Windows.Input;

namespace LazuriteUI.Windows.Main
{
    /// <summary>
    /// Логика взаимодействия для FastSwitchWindow.xaml
    /// </summary>
    public partial class FastSwitchWindow : Window, IHardwareVolumeChanger
    {
        public FastSwitchWindow()
        {
            Singleton.Clear<IHardwareVolumeChanger>();
            Singleton.Add(this);
            this.MouseWheel += Window_MouseWheel;
            InitializeComponent();
            switchesGrid.Initialize();
        }

        public event EventsHandler<int> VolumeDown;
        public event EventsHandler<int> VolumeUp;

        private void gridBack_MouseDown(object sender, MouseButtonEventArgs e)
        {
            switchesGrid.Dispose();
            Close();
        }

        private void Window_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta < 0)
                VolumeDown?.Invoke(this, new EventsArgs<int>(-1));
            else
                VolumeUp?.Invoke(this, new EventsArgs<int>(1));
        }
    }
}
