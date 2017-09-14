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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ModbusPluginUI
{
    /// <summary>
    /// Логика взаимодействия для ConfigureTcpTransport.xaml
    /// </summary>
    public partial class ConfigureTcpTransportView : UserControl
    {
        public ConfigureTcpTransportView()
        {
            InitializeComponent();
            tbPort.Validation = (str) => str == "" || ushort.Parse(str) > 0;
            tbReadTimeout.Validation = (str) => ushort.Parse(str) > 100;
            tbWriteTimeout.Validation = (str) => ushort.Parse(str) > 100;
        }

        public ModbusTcpTransport Transport
        {
            get
            {
                return new ModbusTcpTransport() {
                    Host = tbHost.Text,
                    Port = ushort.Parse(tbPort.Text),
                    ReadTimeout = ushort.Parse(tbReadTimeout.Text),
                    WriteTimeout = ushort.Parse(tbWriteTimeout.Text),
                    UseUdp = btUdp.Selected
                };
            }
            set
            {
                tbHost.Text = value.Host;
                tbPort.Text = value.Port.ToString();
                tbReadTimeout.Text = value.ReadTimeout.ToString();
                tbWriteTimeout.Text = value.WriteTimeout.ToString();
                btUdp.Selected = value.UseUdp;
            }
        }
    }
}
