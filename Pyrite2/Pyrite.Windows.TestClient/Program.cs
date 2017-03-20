using Pyrite.Windows.ServiceClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.Text;
using System.Threading.Tasks;

namespace Pyrite.Windows.TestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                if (args.Length == 0)
                {
                    args = new[] {
                    "desktop",
                    "444",
                    "PyriteService.svc",
                    "anton",
                    "123"
                };
                }
                var client = new ServiceClientFactory().GetServer(args[0], ushort.Parse(args[1]), args[2], args[3], args[4]);
                foreach (var info in client.GetScenariosInfo())
                {
                    Console.WriteLine(info.ScenarioId + " " + info.ValueType.HumanFriendlyName);
                }
            }
            catch (Exception e)
            {
                WriteException(e);
                throw e;
            }
            Console.ReadKey();
        }
        
        private static void WriteException(Exception e)
        {
            if (e is FaultException)
            {
                File.AppendAllLines("log.txt", new[] { ((FaultException)e).Code.Name });
            }
            File.AppendAllLines("log.txt", new[] { e.Message + "---" + e.StackTrace + "---", "##" });
            
            if (e.InnerException != null)
                WriteException(e.InnerException);
        }
    }
}
