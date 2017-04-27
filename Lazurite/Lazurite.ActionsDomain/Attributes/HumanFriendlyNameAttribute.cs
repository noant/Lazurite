using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lazurite.ActionsDomain.Attributes
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
