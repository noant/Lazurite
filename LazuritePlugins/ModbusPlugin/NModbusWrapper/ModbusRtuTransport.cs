using Modbus.Device;
using System;
using System.Collections.Generic;
using System.IO.Ports;

namespace NModbusWrapper
{
    public class ModbusRtuTransport : IModbusTransport
    {
        private static readonly Dictionary<string, object> PortLockers = new Dictionary<string, object>();
        private static object GetPortLocker(string port)
        {
            port = port.ToUpper();
            lock (PortLockers)
            {
                if (!PortLockers.ContainsKey(port))
                    PortLockers.Add(port, new object());
                return PortLockers[port];
            }
        }

        protected SerialPort ConfigurePort()
        {
            var port = new SerialPort(ComPort);
            port.BaudRate = PortBaudRate;
            port.DataBits = PortDataBits;
            port.Parity = PortParity;
            port.StopBits = PortStopBits;
            port.ReadTimeout = 5000;
            port.Open();
            return port;
        }

        protected IModbusSerialMaster ConfigureMaster(SerialPort port)
        {
            var master = ModbusSerialMaster.CreateRtu(port);
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
            lock (GetPortLocker(ComPort))
            {
                var port = ConfigurePort();
                var master = ConfigureMaster(port);
                action.Invoke(master);
                port.Dispose();
                master.Dispose();
            }
        }

        public object Execute(Func<IModbusMaster, object> action)
        {
            lock (GetPortLocker(ComPort))
            {
                var port = ConfigurePort();
                var master = ConfigureMaster(port);
                var result = action.Invoke(master);
                port.Dispose();
                master.Dispose();
                return result;
            }
        }

        public override string ToString()
        {
            return string.Format("{0}; {1}; {2}", ComPort, PortBaudRate, PortDataBits);
        }
    }
}
