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
        private IHardwareVolumeChanger _oldChanger;

        public FastSwitchWindow()
        {
            if (Singleton.Any<IHardwareVolumeChanger>())
            {
                _oldChanger = Singleton.Resolve<IHardwareVolumeChanger>();
                Singleton.Clear<IHardwareVolumeChanger>();
            }
            Singleton.Add(this); // Add as IHardwareVolumeChanger

            MouseWheel += Window_MouseWheel;
            Closed += FastSwitchWindow_Closed;
            InitializeComponent();
            switchesGrid.Initialize();
        }

        private void FastSwitchWindow_Closed(object sender, System.EventArgs e)
        {
            Singleton.Clear<IHardwareVolumeChanger>();
            if (_oldChanger != null)
                Singleton.Add(_oldChanger);
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
