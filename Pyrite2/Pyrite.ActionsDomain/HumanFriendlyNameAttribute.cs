using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pyrite.ActionsDomain
{
    public class HumanFriendlyNameAttribute : Attribute
    {
        public HumanFriendlyNameAttribute(string value)
        {
            Value = value;
        }

        public string Value { get; private set; }
    }
}
