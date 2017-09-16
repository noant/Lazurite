using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modbus.Device;
using System.IO.Ports;

namespace NModbusWrapper
{
    public class ModbusRtuTransport : IModbusTransport
    {
        protected SerialPort ConfigurePort()
        {
            SerialPort port = new SerialPort(ComPort);
            port.BaudRate = PortBaudRate;
            port.DataBits = PortDataBits;
            port.Parity = PortParity;
            port.StopBits = PortStopBits;
            port.Open();
            return port;
        }

        protected IModbusSerialMaster ConfigureMaster(SerialPort port)
        {
            IModbusSerialMaster master = null;
            master = ModbusSerialMaster.CreateRtu(port);
            master.Transport.ReadTimeout = ModbusReadTimeout;
            master.Transport.WriteTimeout = ModbusWriteTimeout;
            return master;
        }

        public string ComPort { get; set; } = "COM1";
        public int PortBaudRate { get; set; } = 9600;
        public int PortDataBits { get; set; } = 8;
        public Parity PortParity { get; set; } = Parity.None;
        public StopBits PortStopBits { get; set; } = StopBits.One;
        public int ModbusReadTimeout { get; set; } = 2000;
        public int ModbusWriteTimeout { get; set; } = 2000;

        public void Execute(Action<IModbusMaster> action)
        {
            var port = ConfigurePort();
            var master = ConfigureMaster(port);
            action.Invoke(master);
            port.Dispose();
            master.Dispose();
        }

        public object Execute(Func<IModbusMaster, object> action)
        {
            var port = ConfigurePort();
            var master = ConfigureMaster(port);
            var result = action.Invoke(master);
            port.Dispose();
            master.Dispose();
            return result;
        }

        public override string ToString()
        {
            return string.Format("{0}; {1}; {2};", ComPort, PortBaudRate, PortDataBits);
        }
    }
}
