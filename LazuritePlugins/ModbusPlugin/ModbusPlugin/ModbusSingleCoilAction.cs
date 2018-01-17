using Lazurite.ActionsDomain;
using Lazurite.ActionsDomain.Attributes;
using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.Shared.ActionCategory;
using LazuriteUI.Icons;
using ModbusPluginUI;
using NModbusWrapper;

namespace ModbusPlugin
{
    [HumanFriendlyName("Modbus - чтение и запись ячейки")]
    [SuitableValueTypes(typeof(ToggleValueType))]
    [LazuriteIcon(Icon.NetworkHome)]
    [Category(Category.Control)]
    public class ModbusSingleCoilAction : IAction, IModbusSingleCoilAction
    {
        public NModbusManager Manager { get; set; } = new NModbusManager();

        public byte SlaveAddress { get; set; } = 1;

        public byte CoilAddress { get; set; } = 0;

        public string Caption
        {
            get
            {
                return string.Format("{0}; устройство {1}; ячейка {2}", Manager.Transport.ToString(), SlaveAddress, CoilAddress);
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
        } = new ToggleValueType();

        public event ValueChangedEventHandler ValueChanged;

        public string GetValue(ExecutionContext context)
        {
            try
            {
                return Manager.ReadSingleCoil(SlaveAddress, CoilAddress) ? ToggleValueType.ValueON : ToggleValueType.ValueOFF;
            }
            catch
            {
                return ToggleValueType.ValueOFF;
            }
        }

        public void Initialize()
        {
            //don nothing
        }

        public void SetValue(ExecutionContext context, string value)
        {
            try
            {
                Manager.WriteSingleCoil(SlaveAddress, CoilAddress, value == ToggleValueType.ValueON);
            }
            catch
            {
                // do nothing
            }
        }

        public bool UserInitializeWith(ValueTypeBase valueType, bool inheritsSupportedValues)
        {
            return new SingleCoilActionWindow(this).ShowDialog() ?? false;
        }
    }
}
