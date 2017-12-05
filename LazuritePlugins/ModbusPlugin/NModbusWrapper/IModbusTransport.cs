using Modbus.Device;
using System;

namespace NModbusWrapper
{
    public interface IModbusTransport
    {
        object Execute(Func<IModbusMaster, object> action);
        void Execute(Action<IModbusMaster> action);
    }
}
