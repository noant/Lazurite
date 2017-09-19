using System.Linq;
using System.Collections.Generic;
using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;

namespace WakeOnLanUtils
{
    public static class LanUtils
    {
        public static readonly byte[] StandartIpBase = new byte[] { 192, 168, 0 };

        public static CancellationTokenSource ListAllHosts(byte[] ipBase, Action<Address> itemCallback, Action completeCallback)
        {
            var tokenSource = new CancellationTokenSource();
            for (int index = 0; index <= 255; index++)
            {
                if (tokenSource.IsCancellationRequested)
                {
                    completeCallback?.Invoke();
                    completeCallback = null;
                }
                else
                {
                    var ip = ipBase[0] + "." + ipBase[1] + "." + ipBase[2] + "." + index;

                    var ping = new Ping();
                    ping.PingCompleted += (o, e) =>
                    {
                        if (tokenSource.IsCancellationRequested)
                        {
                            completeCallback?.Invoke();
                            completeCallback = null;
                        }
                        else
                        {
                            var curIndex = index;
                            if (e.Error == null && e.Reply.Status == IPStatus.Success)
                                if (itemCallback != null)
                                {
                                    GetMacAddress(
                                        e.Reply.Address,
                                        (mac) =>
                                        {
                                            itemCallback(new Address()
                                            {
                                                IPAddress = e.Reply.Address,
                                                MacAddress = mac
                                            });
                                        });
                                }
                            if (curIndex == 255)
                                completeCallback?.Invoke();
                        }
                    };
                    ping.SendAsync(ip, null);
                }
            }
            return tokenSource;
        }

        private static void GetMacAddress(System.Net.IPAddress address, Action<PhysicalAddress> callback)
        {
            Task.Factory.StartNew(() => {
                try
                {
                    var destAddr = BitConverter.ToInt32(address.GetAddressBytes(), 0);
                    var srcAddr = BitConverter.ToInt32(System.Net.IPAddress.Any.GetAddressBytes(), 0);
                    var macAddress = new byte[6];
                    var macAddrLen = macAddress.Length;
                    var ret = SendArp(destAddr, srcAddr, macAddress, ref macAddrLen);
                    if (ret != 0)
                        throw new System.ComponentModel.Win32Exception(ret);
                    var mac = new PhysicalAddress(macAddress);
                    callback?.Invoke(mac);
                }
                catch
                {
                    //do nothing
                }
            });
        }

        public static byte[] MacAddressParse(string macAddress)
        {
            var strs = macAddress.Split(":".ToCharArray());
            return new byte[] {
                Convert.ToByte(strs[0], 16),
                Convert.ToByte(strs[1], 16),
                Convert.ToByte(strs[2], 16),
                Convert.ToByte(strs[3], 16),
                Convert.ToByte(strs[4], 16),
                Convert.ToByte(strs[5], 16)
            };
        }

        public static string MacAddressParse(byte[] macAddress)
        {
            return string.Format("{0}:{1}:{2}:{3}:{4}:{5}",
                Convert.ToString(macAddress[0]),
                Convert.ToString(macAddress[1]),
                Convert.ToString(macAddress[2]),
                Convert.ToString(macAddress[3]),
                Convert.ToString(macAddress[4]),
                Convert.ToString(macAddress[5]));
        }

        [System.Runtime.InteropServices.DllImport("Iphlpapi.dll", EntryPoint = "SendARP")]
        private extern static int SendArp(Int32 destIpAddress, Int32 srcIpAddress, byte[] macAddress, ref Int32 macAddressLength);

        public struct Address
        {
            public IPAddress IPAddress;
            public PhysicalAddress MacAddress;
        }
    }
}
