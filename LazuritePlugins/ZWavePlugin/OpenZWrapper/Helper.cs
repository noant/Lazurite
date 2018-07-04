using OpenZWaveDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenZWrapper
{
    internal static class Helper
    {
        public static bool SetValueSucceed(ZWManager manager, ZWValueID valueId, ZWValueID.ValueType valueType, object value, string[] possibleValues)
        {
            switch (valueType)
            {
                case ZWValueID.ValueType.Button:
                    return manager.PressButton(valueId);
                    break;
                case ZWValueID.ValueType.Bool:
                    return manager.SetValue(valueId, (bool)value);
                case ZWValueID.ValueType.Byte:
                    return manager.SetValue(valueId, (byte)value);
                case ZWValueID.ValueType.Decimal:
                    return manager.SetValue(valueId, (float)(decimal)value);
                case ZWValueID.ValueType.Int:
                    return manager.SetValue(valueId, (int)value);
                case ZWValueID.ValueType.Short:
                    return manager.SetValue(valueId, (short)value);
                case ZWValueID.ValueType.String:
                    return manager.SetValue(valueId, (string)value);
                case ZWValueID.ValueType.List:
                    return manager.SetValueListSelection(valueId, (string)value);
            }
            return false;
        }

        public static object GetValue(ZWManager manager, ZWValueID valueId, ZWValueID.ValueType valueType, string[] possibleValues)
        {
            switch (valueType)
            {
                case ZWValueID.ValueType.Button:
                    {
                        return string.Empty;
                    }
                case ZWValueID.ValueType.Bool:
                    {
                        manager.GetValueAsBool(valueId, out bool result);
                        return result;
                    }
                case ZWValueID.ValueType.Byte:
                    {
                        manager.GetValueAsByte(valueId, out byte result);
                        return result;
                    }
                case ZWValueID.ValueType.Decimal:
                    {
                        manager.GetValueAsDecimal(valueId, out decimal result);
                        return result;
                    }
                case ZWValueID.ValueType.Int:
                    {
                        manager.GetValueAsInt(valueId, out int result);
                        return result;
                    }
                case ZWValueID.ValueType.Short:
                    {
                        manager.GetValueAsShort(valueId, out short result);
                        return result;
                    }
                case ZWValueID.ValueType.String:
                    {
                        manager.GetValueAsString(valueId, out string result);
                        return result;
                    }
                case ZWValueID.ValueType.List:
                    {
                        manager.GetValueListSelection(valueId, out string result);
                        return result;
                    }
            }
            return null;
        }
    }
}
