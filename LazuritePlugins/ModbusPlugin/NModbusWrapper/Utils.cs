using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NModbusWrapper
{
    public static class Utils
    {
        public static byte[] UShortToBytes(ushort[] data)
        {
            var bytes = new byte[data.Length * 2];
            Buffer.BlockCopy(data, 0, bytes, 0, bytes.Length);
            return bytes;
        }

        public static ushort[] BytesToUShort(byte[] bytes)
        {
            var rem = 0;
            var length = Math.DivRem(bytes.Length, 2, out rem) + rem;
            var ushorts = new ushort[length];
            Buffer.BlockCopy(bytes, 0, ushorts, 0, bytes.Length);
            return ushorts;
        }

        public static ushort[] ToUShort(bool value)
        {
            var bytes = BitConverter.GetBytes(value);
            return BytesToUShort(bytes);
        }

        public static ushort[] ToUShort(short value)
        {
            var bytes = BitConverter.GetBytes(value);
            return BytesToUShort(bytes);
        }

        public static ushort[] ToUShort(ushort value)
        {
            var bytes = BitConverter.GetBytes(value);
            return BytesToUShort(bytes);
        }

        public static ushort[] ToUShort(int value)
        {
            var bytes = BitConverter.GetBytes(value);
            return BytesToUShort(bytes);
        }

        public static ushort[] ToUShort(uint value)
        {
            var bytes = BitConverter.GetBytes(value);
            return BytesToUShort(bytes);
        }

        public static ushort[] ToUShort(long value)
        {
            var bytes = BitConverter.GetBytes(value);
            return BytesToUShort(bytes);
        }

        public static ushort[] ToUShort(ulong value)
        {
            var bytes = BitConverter.GetBytes(value);
            return BytesToUShort(bytes);
        }

        public static ushort[] ToUShort(float value)
        {
            var bytes = BitConverter.GetBytes(value);
            return BytesToUShort(bytes);
        }

        public static ushort[] ToUShort(double value)
        {
            var bytes = BitConverter.GetBytes(value);
            return BytesToUShort(bytes);
        }

        public static ushort[] ToUShort(string value)
        {
            var bytes = Encoding.UTF8.GetBytes(value);
            return BytesToUShort(bytes);
        }

        public static bool GetBool(ushort[] arr)
        {
            var bytes = UShortToBytes(arr);
            return BitConverter.ToBoolean(bytes, 0);
        }

        public static string GetString(ushort[] arr) {
            return Encoding.UTF8.GetString(UShortToBytes(arr)).Trim('\0');
        }

        public static short GetShort(ushort[] arr)
        {
            var bytes = UShortToBytes(arr);
            return BitConverter.ToInt16(bytes, 0);
        }

        public static ushort GetUShort(ushort[] arr)
        {
            var bytes = UShortToBytes(arr);
            return BitConverter.ToUInt16(bytes, 0);
        }

        public static int GetInt(ushort[] arr)
        {
            var bytes = UShortToBytes(arr);
            return BitConverter.ToInt32(bytes, 0);
        }
        
        public static uint GetUInt(ushort[] arr)
        {
            var bytes = UShortToBytes(arr);
            return BitConverter.ToUInt32(bytes, 0);
        }

        public static long GetLong(ushort[] arr)
        {
            var bytes = UShortToBytes(arr);
            return BitConverter.ToInt64(bytes, 0);
        }

        public static ulong GetULong(ushort[] arr)
        {
            var bytes = UShortToBytes(arr);
            return BitConverter.ToUInt64(bytes, 0);
        }

        public static float GetFloat(ushort[] arr)
        {
            var bytes = UShortToBytes(arr);
            return BitConverter.ToSingle(bytes, 0);
        }

        public static double GetDouble(ushort[] arr)
        {
            var bytes = UShortToBytes(arr);
            return BitConverter.ToDouble(bytes, 0);
        }

        public static object ConvertFromUShort(ushort[] arr, Type type)
        {
            if (TypeIs<bool>(type))
                return GetBool(arr);
            if (TypeIs<short>(type))
                return GetShort(arr);
            if (TypeIs<ushort>(type))
                return GetUShort(arr);
            if (TypeIs<int>(type))
                return GetInt(arr);
            if (TypeIs<uint>(type))
                return GetUInt(arr);
            if (TypeIs<long>(type))
                return GetLong(arr);
            if (TypeIs<ulong>(type))
                return GetULong(arr);
            if (TypeIs<float>(type))
                return GetFloat(arr);
            if (TypeIs<double>(type))
                return GetDouble(arr);
            if (TypeIs<string>(type))
                return GetString(arr);
            throw new NotSupportedException("Type [" + type.Name + "] not supported");
        }

        public static ushort[] ConvertToUShort(object obj)
        {
            var type = obj.GetType();
            if (TypeIs<bool>(type))
                return ToUShort((bool)obj);
            if (TypeIs<short>(type))
                return ToUShort((short)obj);
            if (TypeIs<ushort>(type))
                return ToUShort((ushort)obj);
            if (TypeIs<int>(type))
                return ToUShort((int)obj);
            if (TypeIs<uint>(type))
                return ToUShort((uint)obj);
            if (TypeIs<long>(type))
                return ToUShort((long)obj);
            if (TypeIs<ulong>(type))
                return ToUShort((ulong)obj);
            if (TypeIs<float>(type))
                return ToUShort((float)obj);
            if (TypeIs<double>(type))
                return ToUShort((double)obj);
            if (TypeIs<string>(type))
                return ToUShort((string)obj);
            throw new NotSupportedException("Type [" + type.Name + "] not supported");
        }

        private static bool TypeIs<T>(Type type)
        {
            return typeof(T).Equals(type);
        }
    }
}
