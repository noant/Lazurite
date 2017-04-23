using OpenZWrapper;
using Pyrite.ActionsDomain.ValueTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZWavePlugin;

namespace ConsoleTest
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var nodeValuePluginAction = new ZWaveNodeValue();
            nodeValuePluginAction.UserInitializeWith(new FloatValueType());
        }
    }
}
