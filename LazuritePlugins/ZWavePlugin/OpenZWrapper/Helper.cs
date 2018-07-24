using OpenZWave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenZWrapper
{
    internal static class Helper
    {
        public static bool SetValueSucceed(ZWManager manager, ZWValueId valueId, ZWValueType valueType, object value, string[] possibleValues)
        {
            switch (valueType)
            {
                case ZWValueType.Button:
                    return manager.PressButton(valueId);
                case ZWValueType.Bool:
                    return manager.SetValue(valueId, (bool)value);
                case ZWValueType.Byte:
                    return manager.SetValue(valueId, (byte)value);
                case ZWValueType.Decimal:
                    return manager.SetValue(valueId, (float)value);
                case ZWValueType.Int:
                    return manager.SetValue(valueId, (int)value);
                case ZWValueType.Short:
                    return manager.SetValue(valueId, (short)value);
                case ZWValueType.String:
                    return manager.SetValue(valueId, (string)value);
                case ZWValueType.List:
                    return manager.SetValueListSelection(valueId, (string)value);
            }
            return false;
        }
                
        public static object GetValue(ZWManager manager, ZWValueId valueId, ZWValueType valueType, string[] possibleValues)
        {
            switch (valueType)
            {
                case ZWValueType.Button:
                    {
                        return string.Empty;
                    }
                case ZWValueType.Bool:
                    {
                        manager.GetValueAsBool(valueId, out bool result);
                        return result;
                    }
                case ZWValueType.Byte:
                    {
                        manager.GetValueAsByte(valueId, out byte result);
                        return result;
                    }
                case ZWValueType.Decimal:
                    {
                        manager.GetValueAsFloat(valueId, out float result);
                        return result;
                    }
                case ZWValueType.Int:
                    {
                        manager.GetValueAsInt(valueId, out int result);
                        return result;
                    }
                case ZWValueType.Short:
                    {
                        manager.GetValueAsShort(valueId, out short result);
                        return result;
                    }
                case ZWValueType.String:
                    {
                        manager.GetValueAsString(valueId, out string result);
                        return result;
                    }
                case ZWValueType.List:
                    {
                        manager.GetValueListSelection(valueId, out string result);
                        return result;
                    }
            }
            return null;
        }
    }
}
