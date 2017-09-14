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
    public partial class ConfigureRtuTransportView : UserControl
    {
        public ConfigureRtuTransportView()
        {
            InitializeComponent();
            tbSpeed.Validation = (str) => int.Parse(str) >= 75 && int.Parse(str) <= 128000;
            tbDataBits.Validation = (str) => int.Parse(str) >= 4 && int.Parse(str) <= 8;
            tbReadTimeout.Validation = (str) => ushort.Parse(str) > 100;
            tbWriteTimeout.Validation = (str) => ushort.Parse(str) > 100;
        }

        private System.IO.Ports.Parity GetSelectedParity()
        {
            if (lvParity.SelectedItem == lvParity.GetItems()[0])
                return System.IO.Ports.Parity.Even;
            if (lvParity.SelectedItem == lvParity.GetItems()[1])
                return System.IO.Ports.Parity.Mark;
            if (lvParity.SelectedItem == lvParity.GetItems()[2])
                return System.IO.Ports.Parity.None;
            if (lvParity.SelectedItem == lvParity.GetItems()[3])
                return System.IO.Ports.Parity.Odd;
            if (lvParity.SelectedItem == lvParity.GetItems()[4])
                return System.IO.Ports.Parity.Space;

            return System.IO.Ports.Parity.None;
        }

        private void SelectParity(System.IO.Ports.Parity parity)
        {
            if (parity == System.IO.Ports.Parity.Even)
                lvParity.SelectedItem = lvParity.GetItems()[0];
            else if (parity == System.IO.Ports.Parity.Mark)
                lvParity.SelectedItem = lvParity.GetItems()[1];
            else if (parity == System.IO.Ports.Parity.None)
                lvParity.SelectedItem = lvParity.GetItems()[2];
            else if (parity == System.IO.Ports.Parity.Odd)
                lvParity.SelectedItem = lvParity.GetItems()[3];
            else if (parity == System.IO.Ports.Parity.Space)
                lvParity.SelectedItem = lvParity.GetItems()[4];
        }

        private void SelectStopBits(System.IO.Ports.StopBits stopBits)
        {
            if (stopBits == System.IO.Ports.StopBits.None)
                lvStopBits.SelectedItem = lvStopBits.GetItems()[0];
            else if (stopBits == System.IO.Ports.StopBits.One)
                lvStopBits.SelectedItem = lvStopBits.GetItems()[1];
            else if (stopBits == System.IO.Ports.StopBits.OnePointFive)
                lvStopBits.SelectedItem = lvStopBits.GetItems()[2];
            else if (stopBits == System.IO.Ports.StopBits.Two)
                lvStopBits.SelectedItem = lvStopBits.GetItems()[3];
        }

        private System.IO.Ports.StopBits GetSelectedStopBits()
        {
            if (lvStopBits.SelectedItem == lvStopBits.GetItems()[0])
                return System.IO.Ports.StopBits.None;
            if (lvStopBits.SelectedItem == lvStopBits.GetItems()[1])
                return System.IO.Ports.StopBits.One;
            if (lvStopBits.SelectedItem == lvStopBits.GetItems()[2])
                return System.IO.Ports.StopBits.OnePointFive;
            if (lvStopBits.SelectedItem == lvStopBits.GetItems()[3])
                return System.IO.Ports.StopBits.Two;

            return System.IO.Ports.StopBits.One;
        }

        public ModbusRtuTransport Transport
        {
            get
            {
                return new ModbusRtuTransport() {
                    ComPort = tbComPort.Text,
                    ModbusReadTimeout = int.Parse(tbReadTimeout.Text),
                    ModbusWriteTimeout = int.Parse(tbWriteTimeout.Text),
                    PortBaudRate = int.Parse(tbSpeed.Text),
                    PortDataBits = int.Parse(tbDataBits.Text),
                    PortParity = GetSelectedParity(),
                    PortStopBits = GetSelectedStopBits()
                };
            }
            set
            {
                tbComPort.Text = value.ComPort;
                tbDataBits.Text = value.PortDataBits.ToString();
                tbReadTimeout.Text = value.ModbusReadTimeout.ToString();
                tbWriteTimeout.Text = value.ModbusWriteTimeout.ToString();
                tbSpeed.Text = value.PortBaudRate.ToString();
                SelectParity(value.PortParity);
                SelectStopBits(value.PortStopBits);
            }
        }
    }
}
