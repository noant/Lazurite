using NModbusWrapper;

namespace ModbusPluginUI
{
    internal interface ITransportView
    {
        IModbusTransport Transport { get; set; }
    }
}