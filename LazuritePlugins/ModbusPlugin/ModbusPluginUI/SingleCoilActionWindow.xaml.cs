using NModbusWrapper;
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
using System.Windows.Shapes;

namespace ModbusPluginUI
{
    /// <summary>
    /// Логика взаимодействия для SingleCoilActionWindow.xaml
    /// </summary>
    public partial class SingleCoilActionWindow : Window
    {
        public SingleCoilActionWindow(IModbusSingleCoilAction action)
        {
            InitializeComponent();
            actionView.RefreshWith(action);
            actionView.OkPressed += (a) => {
                this.DialogResult = true;
            };
            actionView.CancelPressed += () => {
                this.DialogResult = false;
            };
        }
    }
}
