using System.Windows;

namespace MediaHost.LazuritePlugin.Test
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var plugin = new MediaHostPlugin();

            plugin.UserInitializeWith(null, false);

            plugin.SetValue(null, @"http://spacetv.in/playlist/D47RSEQWWRI/siptv");

            //var plugin1 = new MediaHostPlugin();

            //plugin1.UserInitializeWith(new ToggleValueType(), false);

            //plugin1.SetValue(null, ToggleValueType.ValueON);

            //var plugin2 = new MediaHostPlugin();

            //plugin2.UserInitializeWith(new StateValueType(), false);

            //plugin2.SetValue(null, plugin2.ValueType.AcceptedValues[0]);
        }
    }
}