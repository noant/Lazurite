using Lazurite.IOC;
using Lazurite.Logging;
using Lazurite.MainDomain;
using System;

namespace LazuriteMobile.App
{
    public class DeviceDataHandler : IAddictionalDataHandler
    {
        private static readonly ILogger Log = Singleton.Resolve<ILogger>();

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
            try
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
            catch (Exception e)
            {
                data.Set(new DeviceInfo()
                {
                    Name = string.Format("[UnknownDevice][error: {0}]", e.Message)
                });
                Log.Error("Error while getting device info", e);
            }
        }
    }
}
