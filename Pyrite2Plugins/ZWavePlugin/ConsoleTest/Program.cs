using OpenZWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //var value = "1a23a45a67".GetType().GetMethod("Replace").Invoke("1a23a45a67",  "a", "" ).ToString();
            //ZWaveManager.adad();
            var manager = new ZWaveManager();
            manager.ControllerAdded += (o, e) => Console.WriteLine(string.Format("controller {0} added is {1}", e.Controller.Path, e.Successful));
            manager.ManagerInitialized += (o, e) =>
            {
                Console.WriteLine(string.Format("manager initialized is {0}", e.Successful));
            };
            
            manager.Initialize();

            Thread.Sleep(20000);
            manager.ResetController(manager.GetControllers()[0]);

            var nodes = manager.GetNodes();
            Thread.Sleep(20000);
        }
    }
}
