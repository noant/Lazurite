using Lazurite.ActionsDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.ActionsDomain.Attributes;
using LazuriteUI.Icons;

namespace ModbusPlugin
{
    [HumanFriendlyName("ZWave устройство")]
    [SuitableValueTypes(typeof(FloatValueType), typeof(InfoValueType))]
    [LazuriteIcon(Icon.NetworkHome)]
    public class ModbusRegistersAction : IAction
    {
        public NModbusManager Manager { get; set; }

        public byte SlaveAddress { get; set; } = 1;

        public byte RegisterAddress { get; set; } = 0;

        public ushort WriteReadLength { get; set; } = 0;

        public RegistersMode RegistersMode { get; set; } = RegistersMode.Holding;

        public ValueType ModbusValueType { get; set; } = ModbusPlugin.ValueType.Float;

        public string Caption
        {
            get
            {
                return "Modbus - чтение и запись регистров";
            }

            set
            {
                // do nothing
            }
        }

        public bool IsSupportsEvent
        {
            get
            {
                return false;
            }
        }

        public bool IsSupportsModification
        {
            get
            {
                return true;
            }
        }

        public FloatValueType FloatValueType { get; set; } = new FloatValueType();
        public InfoValueType InfoValueType { get; set; } = new InfoValueType();

        public ValueTypeBase ValueType
        {
            get {
                if (ModbusValueType == ModbusPlugin.ValueType.String)
                    return InfoValueType;
                else
                    return FloatValueType;
            }
            set {
                //do nothing
            }
        }

        public event ValueChangedDelegate ValueChanged;

        public string GetValue(ExecutionContext context)
        {
            try
            {
                if (RegistersMode == RegistersMode.Input)
                    return Manager
                        .ReadInputRegisters(SlaveAddress, RegisterAddress, GetTypeByEnum(ModbusValueType), ModbusValueType == ModbusPlugin.ValueType.String ? WriteReadLength : (ushort)0)
                        .ToString();
                else
                    return Manager
                        .ReadHoldingRegisters(SlaveAddress, RegisterAddress, GetTypeByEnum(ModbusValueType), ModbusValueType == ModbusPlugin.ValueType.String ? WriteReadLength : (ushort)0)
                        .ToString();
            }
            catch
            {
                if (ModbusValueType == ModbusPlugin.ValueType.String)
                    return "Error";
                else return "0";
            }
        }

        public void Initialize()
        {
            throw new NotImplementedException();
        }

        public void SetValue(ExecutionContext context, string value)
        {
            try
            {
                if (RegistersMode == RegistersMode.Holding)
                {
                    var modbusValue = Parse(ModbusValueType, value);
                    Manager.WriteHoldingRegisters(SlaveAddress, RegisterAddress, Parse(ModbusValueType, value));
                }
            }
            catch
            {
                // do nothing
            }
        }

        public bool UserInitializeWith(ValueTypeBase valueType, bool inheritsSupportedValues)
        {
            throw new NotImplementedException();
        }

        private Type GetTypeByEnum(ValueType type)
        {
            switch (type)
            {
                case ModbusPlugin.ValueType.Double:
                    return typeof(double);
                case ModbusPlugin.ValueType.Float:
                    return typeof(float);
                case ModbusPlugin.ValueType.Int:
                    return typeof(int);
                case ModbusPlugin.ValueType.Long:
                    return typeof(long);
                case ModbusPlugin.ValueType.Short:
                    return typeof(short);
                case ModbusPlugin.ValueType.String:
                    return typeof(string);
                case ModbusPlugin.ValueType.UInt:
                    return typeof(uint);
                case ModbusPlugin.ValueType.ULong:
                    return typeof(ulong);
                case ModbusPlugin.ValueType.UShort:
                    return typeof(ushort);
                default:
                    throw new NotSupportedException("type not [" + type + "] supported");
            }
        }

        private object Parse(ValueType type, string value)
        {
            switch (type)
            {
                case ModbusPlugin.ValueType.Double:
                    return double.Parse(value);
                case ModbusPlugin.ValueType.Float:
                    return float.Parse(value);
                case ModbusPlugin.ValueType.Int:
                    return int.Parse(value);
                case ModbusPlugin.ValueType.Long:
                    return long.Parse(value);
                case ModbusPlugin.ValueType.Short:
                    return short.Parse(value);
                case ModbusPlugin.ValueType.String:
                    return value;
                case ModbusPlugin.ValueType.UInt:
                    return uint.Parse(value);
                case ModbusPlugin.ValueType.ULong:
                    return ulong.Parse(value);
                case ModbusPlugin.ValueType.UShort:
                    return ushort.Parse(value);
                default:
                    throw new NotSupportedException("type not [" + type + "] supported");
            }
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
