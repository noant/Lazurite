using Lazurite.MainDomain;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazuriteMobile.App
{
    public class AddictionalDataManager
    {
        private bool IsLocationAvailable()
        {
            if (!CrossGeolocator.IsSupported)
                return false;

            return CrossGeolocator.Current.IsGeolocationAvailable;
        }

        private Position GetGeolocation()
        {
            var task = CrossGeolocator.Current.GetPositionAsync();
            task.Wait();
            return task.Result;
        }

        public AddictionalData Prepare()
        {
            var data = new AddictionalData();
            data.Set("test_cl", "test_cl_val");
            if (IsLocationAvailable())
                data.Set(GetGeolocation());
            return data;
        }

        public void Handle(AddictionalData data)
        {
            var data1 = data;
        }
    }
}
