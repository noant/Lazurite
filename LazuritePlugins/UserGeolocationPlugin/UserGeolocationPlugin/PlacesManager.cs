using Lazurite.Data;
using Lazurite.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserGeolocationPlugin
{
    public class PlacesManager
    {
        private static readonly PluginsDataManagerBase DataManager = Singleton.Resolve<PluginsDataManagerBase>();

        public static PlacesManager Current { get; private set; }

        static PlacesManager() => Load();

        public static void Save() => DataManager.Set(nameof(PlacesManager), Current);

        public static GeolocationPlace[] GetAllAvailablePlaces()
        {
            return Current.Places
                    .Union(new[] { GeolocationPlace.Other, GeolocationPlace.Empty })
                    .ToArray();
        }

        public static void Load()
        {
            try
            {
                if (DataManager.Has(nameof(PlacesManager)))
                    Current = DataManager.Get<PlacesManager>(nameof(PlacesManager));
            }
            catch
            {
                //do nothing
            }
            finally
            {
                if (Current == null)
                    Current = new PlacesManager() {
                        Places = new List<GeolocationPlace>()
                    };
            }
        }

        public List<GeolocationPlace> Places { get; set; }
    }
}
