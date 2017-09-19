using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace WakeOnLanPlugin
{
    public class UdpClientExt : UdpClient
    {
        public UdpClientExt()
            : base()
        {
        }

        private void SetClientToBrodcastMode()
        {
            if (this.Active)
                this.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 0);
        }

        public void WakeOnLan(string macAddress, ushort tryCnt, ushort port)
        {
            var macAddrBytes = macAddress
                .Split("-:".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                .Select(x => byte.Parse(x, NumberStyles.HexNumber, CultureInfo.InvariantCulture));

            WakeOnLan(macAddrBytes.ToArray(), tryCnt, port);
        }

        public void WakeOnLan(byte[] macAddress, ushort tryCnt, ushort port)
        {
            this.Connect(new IPAddress(0xffffffff), port);
            this.SetClientToBrodcastMode();

            var bytes = new List<byte>() { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };

            for (int i = 0; i < 16; i++)
                bytes.AddRange(macAddress);

            for (int i = 0; i < tryCnt; i++)
                this.Send(bytes.ToArray(), bytes.Count);
        }

        public void WakeOnLan(string macAddress)
        {
            WakeOnLan(macAddress, StandartWOLTryCount, StandartWakeOnLanPort);
        }

        public void WakeOnLan(byte[] macAddress)
        {
            WakeOnLan(macAddress, StandartWOLTryCount, StandartWakeOnLanPort);
        }

        public static void SendWakeOnLan(string macAddress)
        {
            new UdpClientExt().WakeOnLan(macAddress);
        }

        public static void SendWakeOnLan(byte[] macAddress)
        {
            new UdpClientExt().WakeOnLan(macAddress);
        }

        public static void SendWakeOnLan(string macAddress, ushort tryCnt, ushort port)
        {
            new UdpClientExt().WakeOnLan(macAddress, StandartWOLTryCount, StandartWakeOnLanPort);
        }

        public static void SendWakeOnLan(byte[] macAddress, ushort tryCnt, ushort port)
        {
            new UdpClientExt().WakeOnLan(macAddress, StandartWOLTryCount, StandartWakeOnLanPort);
        }

        public static readonly ushort StandartWakeOnLanPort = 9;
        public static readonly ushort StandartWOLTryCount = 10;
    }
}
