using ModbusPlugin;
using System;
using System.Collections.Generic;
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
            var ushortArr = new ushort[] {
                26948,
                8308,
                29545,
                29728,
                27493,
                29811,
                0,0,0,0,0,0
            };
            var strRes = Utils.GetString(ushortArr);
            ushortArr = Utils.ConvertToUShort("фыфыфыф asasasas!!!$");
            var result = Utils.ConvertFromUShort(ushortArr, typeof(string));
        }
    }
}
