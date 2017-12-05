using System;

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
