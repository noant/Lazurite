namespace NModbusWrapper
{
    public interface IModbusSingleCoilAction
    {
        NModbusManager Manager { get; set; }

        byte SlaveAddress { get; set; }

        byte CoilAddress { get; set; }
    }
}
