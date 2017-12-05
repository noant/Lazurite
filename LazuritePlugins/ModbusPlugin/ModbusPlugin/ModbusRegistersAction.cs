using Lazurite.ActionsDomain;
using Lazurite.ActionsDomain.Attributes;
using Lazurite.ActionsDomain.ValueTypes;
using LazuriteUI.Icons;
using ModbusPluginUI;
using NModbusWrapper;
using System;

namespace ModbusPlugin
{
    [HumanFriendlyName("Modbus - чтение и запись регистров")]
    [SuitableValueTypes(typeof(FloatValueType), typeof(InfoValueType))]
    [LazuriteIcon(Icon.NetworkHome)]
    public class ModbusRegistersAction : IAction, IModbusRegistersAction
    {
        public NModbusManager Manager { get; set; } = new NModbusManager();

        public byte SlaveAddress { get; set; } = 1;

        public byte RegisterAddress { get; set; } = 0;

        public ushort WriteReadLength { get; set; } = 1;

        public RegistersMode RegistersMode { get; set; } = RegistersMode.Holding;

        public NModbusWrapper.ValueType ModbusValueType { get; set; } = NModbusWrapper.ValueType.Float;

        public string Caption
        {
            get
            {
                return string.Format("{0}; устройство {1}; ячейка {2}", Manager.Transport.ToString(), SlaveAddress, RegisterAddress);
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
        
        public ValueTypeBase ValueType
        {
            get;
            set;
        } = new FloatValueType() { AcceptedValues = new[] { double.MinValue.ToString(), double.MaxValue.ToString() } };

        public event ValueChangedEventHandler ValueChanged;

        public string GetValue(ExecutionContext context)
        {
            try
            {
                if (RegistersMode == RegistersMode.Input)
                    return Manager
                        .ReadInputRegisters(SlaveAddress, RegisterAddress, GetTypeByEnum(ModbusValueType), ModbusValueType == NModbusWrapper.ValueType.String ? WriteReadLength : (ushort)0)
                        .ToString();
                else
                    return Manager
                        .ReadHoldingRegisters(SlaveAddress, RegisterAddress, GetTypeByEnum(ModbusValueType), ModbusValueType == NModbusWrapper.ValueType.String ? WriteReadLength : (ushort)0)
                        .ToString();
            }
            catch
            {
                if (ModbusValueType == NModbusWrapper.ValueType.String)
                    return "Error";
                else return "0";
            }
        }

        public void Initialize()
        {
            //do nothing
        }

        public void SetValue(ExecutionContext context, string value)
        {
            try
            {
                var val = Parse(ModbusValueType, value);
                if (RegistersMode == RegistersMode.Holding)
                {
                    var modbusValue = Parse(ModbusValueType, value);
                    Manager.WriteHoldingRegisters(SlaveAddress, RegisterAddress, val);
                }
            }
            catch
            {
                //do nothing
            }
        }

        public bool UserInitializeWith(ValueTypeBase valueType, bool inheritsSupportedValues)
        {
            var mode = ValueTypeMode.All;
            if (valueType != null)
            {
                if (valueType is InfoValueType)
                    mode = ValueTypeMode.String;
                else
                    mode = ValueTypeMode.Numeric;
            }
            var window = new RegistersActionWindow(this, mode);
            return window.ShowDialog() ?? false;
        }

        private Type GetTypeByEnum(NModbusWrapper.ValueType type)
        {
            switch (type)
            {
                case NModbusWrapper.ValueType.Double:
                    return typeof(double);
                case NModbusWrapper.ValueType.Float:
                    return typeof(float);
                case NModbusWrapper.ValueType.Int:
                    return typeof(int);
                case NModbusWrapper.ValueType.Long:
                    return typeof(long);
                case NModbusWrapper.ValueType.Short:
                    return typeof(short);
                case NModbusWrapper.ValueType.String:
                    return typeof(string);
                case NModbusWrapper.ValueType.UInt:
                    return typeof(uint);
                case NModbusWrapper.ValueType.ULong:
                    return typeof(ulong);
                case NModbusWrapper.ValueType.UShort:
                    return typeof(ushort);
                default:
                    throw new NotSupportedException("type [" + type + "] not supported");
            }
        }

        private object Parse(NModbusWrapper.ValueType type, string value)
        {
            switch (type)
            {
                case NModbusWrapper.ValueType.Double:
                    return double.Parse(value);
                case NModbusWrapper.ValueType.Float:
                    return float.Parse(value);
                case NModbusWrapper.ValueType.Int:
                    return int.Parse(value);
                case NModbusWrapper.ValueType.Long:
                    return long.Parse(value);
                case NModbusWrapper.ValueType.Short:
                    return short.Parse(value);
                case NModbusWrapper.ValueType.String:
                    return value;
                case NModbusWrapper.ValueType.UInt:
                    return uint.Parse(value);
                case NModbusWrapper.ValueType.ULong:
                    return ulong.Parse(value);
                case NModbusWrapper.ValueType.UShort:
                    return ushort.Parse(value);
                default:
                    throw new NotSupportedException("type [" + type + "] not supported");
            }
        }
    }
}
