using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Lazurite.MainDomain
{
    public class AddictionalData: Dictionary<string, string>
    {
        public void Set(string key, string value)
        {
            if (this.ContainsKey(key))
                this[key] = value;
            else this.Add(key, value);
        }
    }
}
