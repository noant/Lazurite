using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenZWrapper
{
    public class Controller
    {
        public string Path { get; set; }
        public bool IsHID { get; set; }
        internal uint HomeID { get; set; }

        public bool Failed { get; set; }

        public override int GetHashCode()
        {
            return Path.GetHashCode() ^ IsHID.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return this.GetHashCode() == obj?.GetHashCode();
        }
    }
}
