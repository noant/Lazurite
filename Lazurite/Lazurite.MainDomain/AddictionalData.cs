using System.Collections.Generic;

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
