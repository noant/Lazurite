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
        
        private CallbacksPool _callbacksPool = new CallbacksPool();

        private ZWManager _manager;

        private List<Controller> _controllers = new List<Controller>();
        private List<Node> _nodes = new List<Node>();

        public ManagerInitializedCallbacksPool ManagerInitializedCallbacksPool
        {
            get;
            private set;
        } = new ManagerInitializedCallbacksPool();

        public bool Initialized { get; private set; } = false;

        public bool IsActive { get; private set; } = false;

        public Controller[] GetControllers()
        {
            return _controllers.ToArray();
        }

        public Node[] GetNodes()
        {
            return _nodes.ToArray();
        }

        public void AddController(Controller controller, Action<bool> callback)
        {
            controller.Path = controller.Path.ToUpper();
            if (!_controllers.Any(x => x.Equals(controller)))
            {
                _controllers.Add(controller);
                var hobj = new HObject(_controllersInfoPath);
                hobj.Zero = _controllers;
                hobj.SaveToFile();
                if (_manager != null)
                    _callbacksPool.ExecuteBool(() =>_manager.AddDriver(controller.Path, controller.IsHID ? ZWControllerInterface.Hid : ZWControllerInterface.Serial), callback);
                else Initialize();
            }
            else
                callback(true);
        }

        public void RemoveController(Controller controller, Action<bool> callback)
        {
            if (_controllers.Any(x => x.Equals(controller)))
            {
                _callbacksPool.Add(callback);
                _controllers.RemoveAll(x => x.Equals(controller));
                if (_manager.RemoveDriver(controller.Path))
                {
                    var hobj = new HObject(_controllersInfoPath);
                    hobj.Zero = _controllers;
                    hobj.SaveToFile();
                }
                else
                {
                    callback(false);
                }
            }
            else
                callback(false);
        }

        public Node GetControllerNode(Controller controller)
        {
            var nodeId = _manager.GetControllerNodeId(controller.HomeID);
            return GetNodes().FirstOrDefault(x => x.Id.Equals(nodeId));
        }

        /// <summary>
        /// Initialize manager if not initialized and wait for all nodes and values added
        /// </summary>
        public void WaitForInitialized()
        {
            if (!IsActive)
                Initialize();
            while (!Initialized)
                Thread.Sleep(100);
        }
        
        public void UpdateNetwork(Controller controller, Node node, Action<bool> callback)
        {
            _callbacksPool.ExecuteBool(() => _manager.RequestNetworkUpdate(controller.HomeID, node.Id), callback);
        }

        public void ReplaceFailedNode(Controller controller, Node node, Action<bool> callback)
        {
            _callbacksPool.ExecuteBool(() => _manager.ReplaceFailedNode(controller.HomeID, node.Id), callback);
        }

        public void RemoveFailedNode(Controller controller, Node node, Action<bool> callback)
        {
            _callbacksPool.ExecuteBool(() => _manager.RemoveFailedNode(controller.HomeID, node.Id), 
                (success) => {
                    if (success)
                        _nodes.Remove(node);
                    callback?.Invoke(success);
                });
        }

        public void RecieveConfiguration(Controller controller, Action<bool> callback)
        {
            _callbacksPool.ExecuteBool(() => _manager.ReceiveConfiguration(controller.HomeID), callback);
        }

        public void UpdateNodeNeighborList(Controller controller, Node node, Action<bool> callback)
        {
            _callbacksPool.ExecuteBool(() => _manager.RequestNodeNeighborUpdate(controller.HomeID, node.Id), callback);
        }

        public void CheckNodeFailed(Controller controller, Node node, Action<bool> callback)
        {
            _callbacksPool.ExecuteBool(() => _manager.HasNodeFailed(controller.HomeID, node.Id), callback);
        }

        public void AddNewDevice(Controller controller, Action<bool> callback)
        {
            _callbacksPool.ExecuteBool(() => _manager.AddNode(controller.HomeID, false), callback);
        }

        public void AddNewSecureDevice(Controller controller, Action<bool> callback)
        {
            _callbacksPool.ExecuteBool(() => _manager.AddNode(controller.HomeID, true), callback);
        }

        public void ResetController(Controller controller, Action<bool> callback)
        {
            _callbacksPool.Add(callback);
            _manager.SoftReset(controller.HomeID);
        }

        public void EraseAll(Controller controller, Action<bool> callback)
        {
            _callbacksPool.Add(callback);
            _manager.ResetController(controller.HomeID);
        }

        public void HealControllerNetwork(Controller controller)
        {
            _manager.HealNetwork(controller.HomeID, true);
        }

        public void RemoveDevice(Controller controller, Action<bool> callback)
        {
            _callbacksPool.ExecuteBool(() => _manager.RemoveNode(controller.HomeID), callback);
        }

        public void TransferPrimaryRole(Controller controller, Action<bool> callback)
        {
            _callbacksPool.ExecuteBool(() => _manager.TransferPrimaryRole(controller.HomeID), callback);
        }

        public void CreateNewPrimary(Controller controller, Action<bool> callback)
        {
            _callbacksPool.ExecuteBool(() => _manager.CreateNewPrimary(controller.HomeID), callback);
        }

        public bool SupportsCancellation(string methodName)
        {
            switch (methodName)
            {
                case nameof(CheckNodeFailed):
                case nameof(RemoveFailedNode):
                case nameof(ReplaceFailedNode):
                    return false;
                default: return true;
            }
        }

        public void Initialize()
        {
            IsActive = true;
            var hasAnyControllers = LoadControllers();
            SetOptions();
            _manager = new ZWManager();
            _manager.Create();
            foreach (var controller in _controllers)
                _manager.AddDriver(controller.Path, controller.IsHID ? ZWControllerInterface.Hid : ZWControllerInterface.Serial);
            
            _manager.OnNotification = (notification) =>
            {
                var zwevent = (ZWControllerState)notification.GetEvent();

                bool? operationFailed = null;

                switch (zwevent)
                {
                    case ZWControllerState.Cancel:
                    case ZWControllerState.Error:
                    case ZWControllerState.Failed:
                    case ZWControllerState.NodeFailed:
                        operationFailed = true;
                        break;
                    case ZWControllerState.Completed:
                    case ZWControllerState.NodeOK:
                    case ZWControllerState.Normal:
                        operationFailed = false;
                        break;
                }

                if (operationFailed != null)
                    _callbacksPool.Dequeue(!operationFailed.Value,
                        nameof(AddNewDevice),
                        nameof(AddNewSecureDevice),
                        nameof(RemoveDevice),
                        nameof(RemoveController),
                        nameof(AddController),
                        nameof(UpdateNetwork),
                        nameof(EraseAll),
                        nameof(ResetController),
                        nameof(RecieveConfiguration),
                        nameof(CheckNodeFailed),
                        nameof(HealControllerNetwork),
                        nameof(RemoveFailedNode),
                        nameof(ReplaceFailedNode),
                        nameof(TransferPrimaryRole),
                        nameof(CreateNewPrimary),
                        nameof(UpdateNodeNeighborList));

                var homeId = notification.GetHomeId();
                var path =_manager.GetControllerPath(homeId);
                var controller = _controllers.FirstOrDefault(x => x.Path.Equals(path));
                var nodeId = notification.GetNodeId();
                var node = _nodes.FirstOrDefault(x => x.Id.Equals(nodeId) && x.HomeId.Equals(homeId));

                var notificationType = notification.GetType();
                switch (notificationType)
                {
                    case ZWNotification.Type.DriverRemoved:
                        {
                            _nodes.RemoveAll(x => x.HomeId.Equals(homeId));
                            _callbacksPool.Dequeue(true, nameof(RemoveController));
                        }
                        break;
                    case ZWNotification.Type.DriverFailed:
                        {
                            _controllers.Remove(controller);
                            if (!_controllers.Any())
                            {
                                Initialized = false;
                                ManagerInitializedCallbacksPool.ExecuteAll(this);
                            }
                            _callbacksPool.Dequeue(false, 
                                nameof(AddController), 
                                nameof(RemoveController));
                        }
                        break;
                    case ZWNotification.Type.DriverReady:
                        {
                            controller.HomeID = homeId;
                            _callbacksPool.Dequeue(true,
                                nameof(AddController));
                        }
                        break;
                    case ZWNotification.Type.AwakeNodesQueried:
                    case ZWNotification.Type.AllNodesQueriedSomeDead:
                    case ZWNotification.Type.AllNodesQueried:
                        {
                            _nodes.Where(x => !x.Initialized).All(x => x.Failed = true);
                            _manager.WriteConfig(notification.GetHomeId());
                            this.Initialized = true;
                            ManagerInitializedCallbacksPool.ExecuteAll(this);
                        }
                        break;
                    case ZWNotification.Type.NodeAdded:
                    case ZWNotification.Type.NodeNew:
                        {
                            node = new Node(nodeId, homeId, _manager);
                            _nodes.Add(node);
                            node.Controller = controller;
                            _manager.RequestAllConfigParams(node.HomeId, node.Id);
                            _callbacksPool.Dequeue(true, 
                                nameof(AddNewDevice),
                                nameof(AddNewSecureDevice));
                        }
                        break;
                    case ZWNotification.Type.EssentialNodeQueriesComplete:
                    case ZWNotification.Type.NodeQueriesComplete:
                        {
                            node.Initialized = true;
                        }
                        break;
                    case ZWNotification.Type.NodeProtocolInfo:
                    case ZWNotification.Type.NodeNaming:
                        {
                            node.Refresh();
                            node.Failed = false;
                        }
                        break;
                    case ZWNotification.Type.NodeRemoved:
                        {
                            _nodes.Remove(node);
                            _callbacksPool.Dequeue(true,
                                nameof(RemoveDevice));
                        }
                        break;
                    case ZWNotification.Type.ValueAdded:
                        {
                            var value = notification.GetValueID();
                            var nodeValue = new NodeValue(value, node);
                            node.Values.Add(nodeValue);
                            nodeValue.Refresh();
                        }
                        break;
                    case ZWNotification.Type.ValueRefreshed:
                        {
                            var value = notification.GetValueID();
                            var nodeValue = node.Values.FirstOrDefault(x => x.Id.Equals(value.GetId()));
                            nodeValue.Refresh();
                        }
                        break;
                    case ZWNotification.Type.ValueRemoved:
                        {
                            var value = notification.GetValueID();
                            if (node != null)
                            {
                                var nodeValue = node.Values.FirstOrDefault(x => x.Id.Equals(value.GetId()));
                                node.Values.Remove(nodeValue);
                            }
                        }
                        break;
                    case ZWNotification.Type.ValueChanged:
                        {
                            var value = notification.GetValueID();
                            var nodeValue = node.Values.FirstOrDefault(x => x.Id.Equals(value.GetId()));
                            nodeValue.CurrentByte = notification.GetByte();
                            nodeValue.CurrentGroupIdx = notification.GetGroupIdx();
                            nodeValue.InternalSet(Helper.GetValue(_manager, value, nodeValue.ZWValueType, nodeValue.PossibleValues));
                        }
                        break;
                }

                //crutch
                node?.Refresh();
            };

            if (!hasAnyControllers)
            {
                Initialized = true;
                ManagerInitializedCallbacksPool.ExecuteAll(this);
            }
        }

        public bool CancelOperation(Controller controller)
        {
            return _manager.CancelControllerCommand(controller.HomeID);
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
    }
}