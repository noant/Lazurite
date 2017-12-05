namespace NModbusWrapper
{
    public interface IModbusRegistersAction
    {
        NModbusManager Manager { get; set; }

        byte SlaveAddress { get; set; }

        byte RegisterAddress { get; set; }

        ushort WriteReadLength { get; set; }

        RegistersMode RegistersMode { get; set; }

        NModbusWrapper.ValueType ModbusValueType { get; set; }

        Lazurite.ActionsDomain.ValueTypes.ValueTypeBase ValueType { get; set; }
    }
}
