using Modbus.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModbusPlugin
{
    public interface IModbusTransport
    {
        object Execute(Func<IModbusMaster, object> action);
        void Execute(Action<IModbusMaster> action);
    }
}
