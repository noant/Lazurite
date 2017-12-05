using System.Windows;
using VolumePlugin;

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
            Utils.SetOutputAudioDevice(3);

            tb.TextChanged += (o, e) => {
                try
                {
                    var deviceNum = int.Parse(tb.Text);
                    Utils.SetOutputAudioDevice(deviceNum);
                    tb1.Text = Utils.GetDefaultOutputDeviceIndex().ToString();
                }
                catch { }
            };

            slider.Value = Utils.GetVolumeLevel();

            slider.ValueChanged += (o, e) => {
                Utils.SetVolumeLevel(e.NewValue);
            };

        }
    }
}
