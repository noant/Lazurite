using HierarchicalData;
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
        private readonly string _controllersInfoPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "controllers.xml");

        private ZWManager _manager;

        private List<Controller> _controllers = new List<Controller>();
        private List<Node> _nodes = new List<Node>();

        public event Action<object, ManagerInitializedEventArgs> ManagerInitialized;
        public event Action<object, ControllerAddedEventArgs> ControllerAdded;

        public bool Initialized { get; private set; } = false;

        public Controller[] GetControllers()
        {
            return _controllers.ToArray();
        }

        public Node[] GetNodes()
        {
            return _nodes.ToArray();
        }

        public void AddController(Controller controller)
        {
            if (!_controllers.Any(x => x.Equals(controller)))
            {
                _controllers.Add(controller);
                var hobj = new HObject(_controllersInfoPath);
                hobj.Zero = _controllers;
                hobj.SaveToFile();
                Initialize();
            }
        }

        public void RemoveController(Controller controller)
        {
            if (_controllers.Any(x => x.Equals(controller)))
            {
                _controllers.RemoveAll(x => x.Equals(controller));
                _manager.RemoveDriver(controller.Path);
                var hobj = new HObject(_controllersInfoPath);
                hobj.Zero = _controllers;
                hobj.SaveToFile();
            }
        }

        private void SetOptions()
        {
            var options = new ZWOptions();
            options.Create(
              Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "config"),
              string.Empty,
              string.Empty);
            options.AddOptionInt("SaveLogLevel", (int)ZWLogLevel.None);
            options.Lock();
        }

        private bool LoadControllers()
        {
            try
            {
                var hobj = HObject.FromFile(_controllersInfoPath);
                if (hobj.Zero is List<Controller>)
                {
                    _controllers = hobj.Zero;
                    return _controllers.Any();
                }
            }
            catch
            {
                //do nothing
            }
            return false;
        }

        public void ResetController(Controller controller)
        {
            Initialized = false;
            ManagerInitialized?.Invoke(this, new ManagerInitializedEventArgs()
            {
                Manager = this
            });
            _manager.ResetController(controller.HomeID);
            _manager.RemoveDriver(controller.Path);
            _manager.AddDriver(controller.Path, controller.IsHID ? ZWControllerInterface.Hid : ZWControllerInterface.Serial);
        }

        public void HealControllerNetwork(Controller controller)
        {
            _manager.HealNetwork(controller.HomeID, true);
        }

        public void Initialize()
        {            
            if (LoadControllers())
            {
                SetOptions();
                _manager = new ZWManager();
                _manager.Create();
                foreach (var controller in _controllers)
                    _manager.AddDriver(controller.Path, controller.IsHID ? ZWControllerInterface.Hid : ZWControllerInterface.Serial);
                
                _manager.OnNotification = (notification) =>
                {
                    var notificationType = notification.GetType();
                    switch (notificationType)
                    {
                        case ZWNotification.Type.DriverRemoved:
                            {
                                var homeId = notification.GetHomeId();
                                _nodes.RemoveAll(x => x.HomeId.Equals(homeId));
                            }
                            break;
                        case ZWNotification.Type.DriverFailed:
                            {
                                var homeId = notification.GetHomeId();
                                var path =_manager.GetControllerPath(homeId);
                                var controller = _controllers.FirstOrDefault(x => x.Path.Equals(path));
                                ControllerAdded?.Invoke(this, new ControllerAddedEventArgs()
                                {
                                    Controller = controller,
                                    Manager = this,
                                    Successful = false
                                });
                                _controllers.Remove(controller);
                                if (!_controllers.Any())
                                {
                                    Initialized = false;
                                    ManagerInitialized?.Invoke(this, new ManagerInitializedEventArgs() {
                                        Manager = this                                        
                                    });
                                }
                            }
                            break;
                        case ZWNotification.Type.DriverReady:
                            {
                                var homeId = notification.GetHomeId();
                                var path =_manager.GetControllerPath(homeId);
                                var controller = _controllers.FirstOrDefault(x => x.Path.Equals(path));
                                controller.HomeID = homeId;
                                ControllerAdded?.Invoke(this, new ControllerAddedEventArgs()
                                {
                                    Controller = controller,
                                    Manager = this,
                                    Successful = true
                                });
                            }
                            break;
                        case ZWNotification.Type.AwakeNodesQueried:
                        case ZWNotification.Type.AllNodesQueriedSomeDead:
                        case ZWNotification.Type.AllNodesQueried:
                            {
                                Initialized = true;
                                _manager.WriteConfig(notification.GetHomeId());
                                ManagerInitialized?.Invoke(this, new ManagerInitializedEventArgs()
                                {
                                    Manager = this
                                });
                            }
                            break;
                        case ZWNotification.Type.NodeAdded:
                            {
                                var node = new Node(notification.GetNodeId(), notification.GetHomeId(), _manager);
                                _nodes.Add(node);
                                _manager.SendNodeInformation(node.HomeId, node.Id);
                            }
                            break;
                        case ZWNotification.Type.NodeQueriesComplete:
                        case ZWNotification.Type.NodeProtocolInfo:
                        case ZWNotification.Type.NodeNaming:
                            {
                                var nodeId = notification.GetNodeId();
                                var homeId = notification.GetHomeId();
                                var node = _nodes.FirstOrDefault(x => x.Id.Equals(nodeId) && x.HomeId.Equals(homeId));
                                node.Refresh();
                            }
                            break;
                        case ZWNotification.Type.NodeRemoved:
                            {
                                var nodeId = notification.GetNodeId();
                                var homeId = notification.GetHomeId();
                                var node = _nodes.FirstOrDefault(x => x.Id.Equals(nodeId) && x.HomeId.Equals(homeId));
                                _nodes.Remove(node);
                            }
                            break;
                        case ZWNotification.Type.ValueAdded:
                            {
                                var nodeId = notification.GetNodeId();
                                var homeId = notification.GetHomeId();
                                var node = _nodes.FirstOrDefault(x => x.Id.Equals(nodeId) && x.HomeId.Equals(homeId));
                                var value = notification.GetValueID();
                                var nodeValue = new NodeValue(value, node);
                                node.Values.Add(nodeValue);
                                nodeValue.Refresh();
                            }
                            break;
                        case ZWNotification.Type.ValueRefreshed:
                            {
                                var nodeId = notification.GetNodeId();
                                var homeId = notification.GetHomeId();
                                var node = _nodes.FirstOrDefault(x => x.Id.Equals(nodeId) && x.HomeId.Equals(homeId));
                                var value = notification.GetValueID();
                                var nodeValue = node.Values.FirstOrDefault(x => x.Id.Equals(value.GetId()));
                                nodeValue.Refresh();
                            }
                            break;
                        case ZWNotification.Type.ValueRemoved:
                            {
                                var nodeId = notification.GetNodeId();
                                var homeId = notification.GetHomeId();
                                var node = _nodes.FirstOrDefault(x => x.Id.Equals(nodeId) && x.HomeId.Equals(homeId));
                                var value = notification.GetValueID();
                                var nodeValue = node.Values.FirstOrDefault(x => x.Id.Equals(value.GetId()));
                                node.Values.Remove(nodeValue);
                            }
                            break;
                        case ZWNotification.Type.ValueChanged:
                            {
                                var nodeId = notification.GetNodeId();
                                var homeId = notification.GetHomeId();
                                var node = _nodes.FirstOrDefault(x => x.Id.Equals(nodeId) && x.HomeId.Equals(homeId));
                                var value = notification.GetValueID();
                                var nodeValue = node.Values.FirstOrDefault(x => x.Id.Equals(value.GetId()));
                                nodeValue.CurrentByte = notification.GetByte();
                                nodeValue.CurrentGroupIdx = notification.GetGroupIdx();
                                nodeValue.InternalSet(Helper.GetValue(_manager, value, nodeValue.ValueType, nodeValue.PossibleValues));
                            }
                            break;
                    }
                };
            }
            else
            {
                ManagerInitialized?.Invoke(this, new ManagerInitializedEventArgs()
                {
                    Manager = this
                });
            }
        }
    }
}
