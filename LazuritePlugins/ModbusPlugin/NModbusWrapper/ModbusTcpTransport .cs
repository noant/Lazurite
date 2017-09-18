using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modbus.Device;
using System.IO.Ports;
using System.Net.Sockets;

namespace NModbusWrapper
{
    public class ModbusTcpTransport : IModbusTransport
    {
        protected IModbusMaster ConfigureMaster(TcpClient client)
        {
            return ModbusIpMaster.CreateIp(client);
        }

        protected TcpClient ConfigureTcpClient()
        {
            var client = new TcpClient(Host, Port);
            client.ReceiveTimeout = WriteTimeout;
            client.SendTimeout = ReadTimeout;
            return client;
        }

        protected UdpClient ConfigureUpdClient()
        {
            var client = new UdpClient(Host, Port);
            return client;
        }

        public object Execute(Func<IModbusMaster, object> action)
        {
            var client = ConfigureTcpClient();
            var master = ConfigureMaster(client);
            var result = action.Invoke(master);
            client.Close();
            master.Dispose();
            return result;
        }

        public void Execute(Action<IModbusMaster> action)
        {
            dynamic client = UseUdp ? (dynamic)ConfigureUpdClient() : ConfigureTcpClient();
            var master = ConfigureMaster(client);
            action.Invoke(master);
            client.Close();
            master.Dispose();
        }

        public string Host { get; set; } = "localhost";
        public ushort Port { get; set; } = 502;
        public int ReadTimeout { get; set; } = 2000;
        public int WriteTimeout { get; set; } = 2000;

        public bool UseUdp { get; set; } = false;

        public override string ToString()
        {
            return string.Format("{0}; {1}; {2}", Host, Port, UseUdp ? "UDP" : "TCP");
        }
    }
}
