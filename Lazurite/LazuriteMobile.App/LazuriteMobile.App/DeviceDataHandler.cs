using Lazurite.IOC;
using Lazurite.Logging;
using Lazurite.MainDomain;
using Plugin.DeviceInfo;
using System;

namespace LazuriteMobile.App
{
    public class DeviceDataHandler : IAddictionalDataHandler
    {
        private static readonly ILogger Log = Singleton.Resolve<ILogger>();

        public void Handle(AddictionalData data, object tag)
        {
            //do nothing
        }

        public void Initialize()
        {
            //do nothing
        }

        public void Prepare(AddictionalData data, object tag)
        {
            try
            {
                var deviceInfo = new DeviceInfo();
                deviceInfo.Name = string.Format(
                    "[Model: {0}]; [DeviceId: {1}]",
                    CrossDeviceInfo.Current.Model,
                    CrossDeviceInfo.Current.Id);
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
