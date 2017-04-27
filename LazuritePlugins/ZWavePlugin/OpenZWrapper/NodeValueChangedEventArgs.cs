using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenZWrapper
{
    public class NodeValueChangedEventArgs : EventArgs
    {
        public NodeValue Value { get; set; }
    }
}
