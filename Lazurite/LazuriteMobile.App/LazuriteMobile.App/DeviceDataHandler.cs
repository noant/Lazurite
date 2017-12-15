using Lazurite.MainDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazuriteMobile.App
{
    public class DeviceDataHandler : IAddictionalDataHandler
    {
        public void Handle(AddictionalData data)
        {
            //do nothing
        }

        public void Initialize()
        {
            //do nothing
        }

        public void Prepare(AddictionalData data)
        {
            var deviceInfo = new DeviceInfo();
            deviceInfo.Name = string.Format(
                "[Model: {0}]; [OS: {1},{2}]; [Manufacturer: {3}]; [DeviceId: {4}]",
                Plugin.DeviceInfo.CrossDevice.Hardware.Model,
                Plugin.DeviceInfo.CrossDevice.Hardware.OperatingSystem,
                Plugin.DeviceInfo.CrossDevice.Hardware.OperatingSystemVersion,
                Plugin.DeviceInfo.CrossDevice.Hardware.Manufacturer,
                Plugin.DeviceInfo.CrossDevice.Hardware.DeviceId);
            data.Set(deviceInfo);
        }
    }
}
