using OpenZWaveDotNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenZWrapper
{
    public class ZWaveManager
    {
        private static void SetOptions()
        {
            var options = new ZWOptions();
            options.Create(
              Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "config"),
              string.Empty,
              string.Empty);
            options.AddOptionInt("SaveLogLevel",
              (int)ZWLogLevel.None);
            options.Lock();
        }
        public static void adad()
        {
            SetOptions();
            var manager = new ZWManager();
            manager.Create();
            manager.AddDriver("\\\\.\\COM4", ZWControllerInterface.Serial);
            var nodes = new List<Node>();
            manager.OnNotification = (notification) =>
            {
                var notificationType = notification.GetType();
                switch (notificationType)
                {
                    case ZWNotification.Type.AllNodesQueriedSomeDead:
                    case ZWNotification.Type.AllNodesQueried:
                        {
                            
                        }
                        break;
                    case ZWNotification.Type.NodeAdded:
                        {
                            var node = new Node(notification.GetNodeId(), notification.GetHomeId(), manager);
                            nodes.Add(node);
                            manager.SendNodeInformation(node.HomeId, node.Id);
                        }
                        break;
                    case ZWNotification.Type.NodeQueriesComplete:
                    case ZWNotification.Type.NodeProtocolInfo:
                    case ZWNotification.Type.NodeNaming:
                        {
                            var nodeId = notification.GetNodeId();
                            var homeId = notification.GetHomeId();
                            var node = nodes.FirstOrDefault(x => x.Id.Equals(nodeId) && x.HomeId.Equals(homeId));
                            node.Refresh();
                        }
                        break;
                    case ZWNotification.Type.ValueAdded:
                        {
                            var nodeId = notification.GetNodeId();
                            var homeId = notification.GetHomeId();
                            var node = nodes.FirstOrDefault(x => x.Id.Equals(nodeId) && x.HomeId.Equals(homeId));
                            var value = notification.GetValueID();
                            var nodeValue = new NodeValue(value, node);
                        }
                        break;
                }
            };

            while (true)
                Thread.Sleep(4000);
        }
    }
}
