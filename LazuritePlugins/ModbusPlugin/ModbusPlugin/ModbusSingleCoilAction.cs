using Lazurite.ActionsDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.ActionsDomain.Attributes;
using LazuriteUI.Icons;
using NModbusWrapper;

namespace ModbusPlugin
{
    [HumanFriendlyName("ZWave устройство")]
    [SuitableValueTypes(typeof(ToggleValueType))]
    [LazuriteIcon(Icon.NetworkHome)]
    public class ModbusSingleCoilAction : IAction
    {
        public NModbusManager Manager { get; set; }

        public byte SlaveAddress { get; set; } = 1;

        public byte CoilAddress { get; set; } = 0;

        public string Caption
        {
            get
            {
                return "Modbus - чтение и запись одной ячейки";
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

        public event ValueChangedDelegate ValueChanged;

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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }
    }
}
