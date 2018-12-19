using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaHost.AverMedia
{
    public class EnumObject<T> where T : Enum
    {
        public EnumObject(T defaultValue = default) {
            Values = Enum.GetNames(typeof(T));
            SelectedValue = Enum.GetName(typeof(T), defaultValue);
        }

        public string[] Values { get; }
        public string SelectedValue { get; set; }
        public T SelectedEnum => (T)Enum.Parse(typeof(T), SelectedValue);
    }
}
