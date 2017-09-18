using Lazurite.ActionsDomain.ValueTypes;
using ModbusPlugin;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
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

namespace TestProject
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Test();
        }

        public void Test()
        {
            //ModbusPluginUI.ConfigureTransportView.Show((t) => {
            //    var transport = t;
            //    t = null;
            //    Test();
            //});

            var action = new ModbusSingleCoilAction();
            var result = action.UserInitializeWith(new ToggleValueType(), false);
            var a = 0;
        }
    }
}
