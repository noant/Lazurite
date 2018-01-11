using Lazurite.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserGeolocationPlugin
{
    public class GeolocationPlace
    {
        public string Name { get; set; }
        public Lazurite.Shared.Geolocation Location { get;set; }
        public int MetersRadius { get; set; }

        public static readonly GeolocationPlace Empty = new GeolocationPlace()
        {
            Name = "Пусто"
        };
        
        public static readonly GeolocationPlace Other = new GeolocationPlace()
        {
            Name = "Другое"
        };
    }
}
