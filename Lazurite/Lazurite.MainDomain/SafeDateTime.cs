using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Lazurite.MainDomain
{
    [DataContract]
    public class SafeDateTime
    {
        [DataMember]
        public ushort Year { get; set; }
        [DataMember]
        public ushort Month { get; set; }
        [DataMember]
        public ushort Day { get; set; }
        [DataMember]
        public ushort Hour { get; set; }
        [DataMember]
        public ushort Minute { get; set; }
        [DataMember]
        public ushort Second { get; set; }
        [DataMember]
        public ushort Millisecond { get; set; }

        public static SafeDateTime FromDateTime(DateTime dateTime)
        {
            return new SafeDateTime()
            {
                Year = (ushort)dateTime.Year,
                Month = (ushort)dateTime.Month,
                Day = (ushort)dateTime.Day,
                Hour = (ushort)dateTime.Hour,
                Minute = (ushort)dateTime.Minute,
                Second = (ushort)dateTime.Second,
                Millisecond = (ushort)dateTime.Millisecond
            };
        }

        public DateTime ToDateTime()
        {
            return new DateTime(Year, Month, Day, Hour, Minute, Second, Millisecond);
        }
    }
}
