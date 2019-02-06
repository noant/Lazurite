using Lazurite.ActionsDomain.Attributes;
using ProtoBuf;
using System;

namespace Lazurite.ActionsDomain.ValueTypes
{
    [HumanFriendlyName("Геолокация")]
    [ProtoContract]
    public class GeolocationValueType : ValueTypeBase
    {
        private static readonly GeolocationData DefaultGeolocationData = new GeolocationData();

        public override ValueTypeInterpreteResult Interprete(string param)
        {
            var strs = param.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            if (strs.Length < 2)
                return new ValueTypeInterpreteResult(false, param);
            else
            {
                if (double.TryParse(strs[0], out double val) &&
                    double.TryParse(strs[1], out val) &&
                    (strs.Length < 3 || DateTime.TryParse(strs[2], out DateTime dt)))
                    return new ValueTypeInterpreteResult(true, param);
                else return new ValueTypeInterpreteResult(false, param);
            }
        }

        public override bool CanBeModified => false;

        public override string HumanFriendlyName => "Геолокация";

        public override bool SupportsNumericalComparisons => false;

        public override string DefaultValue => DefaultGeolocationData.ToString();
    }

    public struct GeolocationData
    {
        public DateTime DateTime { get; private set; }
        public double Latitude { get; private set; }
        public double Longtitude { get; private set; }

        public GeolocationData(double lat, double lng, DateTime dateTime)
        {
            DateTime = dateTime;
            Latitude = lat;
            Longtitude = lng;
        }

        public bool IsEmpty
        {
            get
            {
                return
                double.IsNaN(Latitude) ||
                double.IsInfinity(Latitude) ||
                double.IsNaN(Longtitude) ||
                double.IsInfinity(Longtitude);
            }
        }

        public override string ToString()
        {
            return string.Format("{0};{1};{2};", Latitude, Longtitude, DateTime);
        }

        public static GeolocationData FromString(string str)
        {
            var strs = str.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            return new GeolocationData(double.Parse(strs[0]), double.Parse(strs[1]), DateTime.Parse(strs[2]));
        }
    }
}
