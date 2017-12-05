using LazuriteUI.Windows.Controls;
using NModbusWrapper;
using System.Windows.Controls;

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
            tbHost.Validation = (v) =>
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
