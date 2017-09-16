using LazuriteUI.Windows.Controls;
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
    public partial class ConfigureTcpTransportView : UserControl, ITransportView
    {
        public ConfigureTcpTransportView()
        {
            InitializeComponent();
            tbPort.Validation = EntryViewValidation.UShortValidation();
            tbReadTimeout.Validation = EntryViewValidation.IntValidation(min: 100);
            tbWriteTimeout.Validation = EntryViewValidation.IntValidation(min: 100);
            tbHost.Validation = (s, v) =>
            {
                if (v.InputString == "")
                {
                    v.OutputString = "localhost";
                    v.SelectAll = true;
                }
            };
        }

        public IModbusTransport Transport
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
                var val = (ModbusTcpTransport)value;
                tbHost.Text = val.Host;
                tbPort.Text = val.Port.ToString();
                tbReadTimeout.Text = val.ReadTimeout.ToString();
                tbWriteTimeout.Text = val.WriteTimeout.ToString();
                btUdp.Selected = val.UseUdp;
            }
        }
    }
}
