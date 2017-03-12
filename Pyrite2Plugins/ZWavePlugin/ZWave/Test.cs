using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWave.Channel;
using ZWave.CommandClasses;

namespace ZWave
{
    public class Test
    {
        public async void adad()
        {
            var controller = new ZWaveController("COM1");
            //var nodes = await controller.GetNodes();
            //var a = await nodes.First().GetSupportedCommandClasses();
            //controller.
            //var classes = await nodes.First().GetSupportedCommandClasses();
            //var class1 = nodes.First().GetCommandClass<SwitchBinary>();
            //class1.Set()
            controller.Channel.NodeEventReceived += (o, e) => {

            };

            controller.Channel.Send(Function.RemoveNodeFromNetwork,);
        }
    }
}
