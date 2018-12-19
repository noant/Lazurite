using Lazurite.ActionsDomain.ValueTypes;
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

            //var plugin = new MediaHostPlugin();

            //plugin.UserInitializeWith(null, false);

            //plugin.SetValue(null, @"D:\Other\IpTvPlaylist\tvoetv.m3u8");

            var plugin1 = new MediaHostPlugin();

            plugin1.UserInitializeWith(new ToggleValueType(), false);

            plugin1.SetValue(null, ToggleValueType.ValueON);

            var plugin2 = new MediaHostPlugin();

            plugin2.UserInitializeWith(new StateValueType(), false);

            plugin2.SetValue(null, plugin2.ValueType.AcceptedValues[0]);
        }
    }
}
