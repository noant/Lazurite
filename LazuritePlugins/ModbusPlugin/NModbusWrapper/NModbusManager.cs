using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace NModbusWrapper
{
    public class NModbusManager
    {
        public IModbusTransport Transport { get; set; } = new ModbusRtuTransport();

        public bool ReadSingleCoil(byte slaveAddress, ushort coilAddress)
        {
            return (bool)Transport.Execute((master) => master.ReadCoils(slaveAddress, coilAddress, 1).First());
        }

        public void WriteSingleCoil(byte slaveAddress, ushort coilAddress, bool value)
        {
            Transport.Execute((master) => master.WriteSingleCoil(slaveAddress, coilAddress, value));
        }

        public object ReadHoldingRegisters(byte slaveAddress, ushort startAddress, Type type, ushort length=0)
        {
            if (length == 0)
                length = (ushort)(Marshal.SizeOf(type) / 2);
            var result = (ushort[])Transport.Execute((master) => master.ReadHoldingRegisters(slaveAddress, startAddress, length));
            return Utils.ConvertFromUShort(result, type);
        }
        
        public object ReadInputRegisters(byte slaveAddress, ushort startAddress, Type type, ushort length = 0)
        {
            if (length == 0)
                length = (ushort)(Marshal.SizeOf(type) / 2);
            var result = (ushort[])Transport.Execute((master) => master.ReadInputRegisters(slaveAddress, startAddress, length));
            return Utils.ConvertFromUShort(result, type);
        }

        public void WriteHoldingRegisters(byte slaveAddress, ushort startAddress, object value)
        {
            Transport.Execute((master) => master.WriteMultipleRegisters(slaveAddress, startAddress, Utils.ConvertToUShort(value)));
        }
    }
    
    public enum RegistersMode
    {
        Holding,
        Input
    }

    public enum ValueType
    {
        Short,
        UShort,
        Int,
        UInt,
        Long,
        ULong,
        Float,
        Double,
        String
    }
}
