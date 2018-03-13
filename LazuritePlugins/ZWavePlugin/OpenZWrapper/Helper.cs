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
                        bool result;
                        manager.GetValueAsBool(valueId, out result);
                        return result;
                    }
                case ZWValueID.ValueType.Byte:
                    {
                        byte result;
                        manager.GetValueAsByte(valueId, out result);
                        return result;
                    }
                case ZWValueID.ValueType.Decimal:
                    {
                        decimal result;
                        manager.GetValueAsDecimal(valueId, out result);
                        return result;
                    }
                case ZWValueID.ValueType.Int:
                    {
                        int result;
                        manager.GetValueAsInt(valueId, out result);
                        return result;
                    }
                case ZWValueID.ValueType.Short:
                    {
                        short result;
                        manager.GetValueAsShort(valueId, out result);
                        return result;
                    }
                case ZWValueID.ValueType.String:
                    {
                        string result;
                        manager.GetValueAsString(valueId, out result);
                        return result;
                    }
                case ZWValueID.ValueType.List:
                    {
                        string result;
                        manager.GetValueListSelection(valueId, out result);
                        return result;
                    }
            }
            return null;
        }
    }
}
