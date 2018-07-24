using Lazurite.Data;
using Lazurite.IOC;
using Lazurite.Shared;
using OpenZWave;
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
        private readonly string _key = "zwave_controllers";
        private CallbacksPool _callbacksPool = new CallbacksPool();
        private ZWManager _manager;
        private List<Controller> _controllers = new List<Controller>();
        private List<Node> _nodes = new List<Node>();
        
        public ManagerInitializedCallbacksPool ManagerInitializedCallbacksPool
        {
            get;
            private set;
        } = new ManagerInitializedCallbacksPool();

        public ZWaveManagerState State { get; private set; } = ZWaveManagerState.None;

        private void SaveControllersList() =>
            Singleton.Resolve<PluginsDataManagerBase>().Set(_key, _controllers);

        public Controller[] GetControllers() => _controllers.ToArray();

        public Node[] GetNodes() => _nodes.ToArray();

        public void AddController(Controller controller, Action<bool> callback)
        {
            lock (_nodes)
            {
                controller.Path = controller.Path.ToUpper();
                if (!_controllers.Any(x => x.Equals(controller)))
                {
                    _controllers.Add(controller);
                    SaveControllersList();
                    if (_manager != null)
                        _callbacksPool.ExecuteBool(() => _manager.AddDriver(controller.Path, controller.IsHID ? ZWControllerInterface.Hid : ZWControllerInterface.Serial),
                            (result) =>
                            {
                                if (!result)
                                {
                                    _controllers.Remove(controller);
                                    SaveControllersList();
                                }
                                callback?.Invoke(result);
                            });
                    else Initialize();
                }
            }
        }

        public void RemoveController(Controller controller, Action<bool> callback)
        {
            if (_controllers.Any(x => x.Equals(controller)))
            {
                _callbacksPool.Add(callback);
                _controllers.RemoveAll(x => x.Equals(controller));
                if (_manager.RemoveDriver(controller.Path))
                    SaveControllersList();
                else
                    callback(false);
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
            lock (_nodes)
            {
                if (State == ZWaveManagerState.None)
                    Initialize();
                while (State == ZWaveManagerState.Initializing)
                    Thread.Sleep(100);
            }
        }
        
        public void UpdateNetwork(Controller controller, Node node, Action<bool> callback)
        {
            _callbacksPool.ExecuteBool(() => _manager.RequestNetworkUpdate(controller.HomeID, node.Id), callback, 60 * 5);
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
            _callbacksPool.ExecuteBool(() => _manager.ReceiveConfiguration(controller.HomeID), callback, 5*60);
        }

        public void UpdateNodeNeighborList(Controller controller, Node node, Action<bool> callback)
        {
            _callbacksPool.ExecuteBool(() => _manager.RequestNodeNeighborUpdate(controller.HomeID, node.Id), callback, 5*60);
        }

        public void CheckNodeFailed(Controller controller, Node node, Action<bool> callback)
        {
            _callbacksPool.ExecuteBool(() => _manager.HasNodeFailed(controller.HomeID, node.Id), callback);
        }

        public void AddNewDevice(Controller controller, Action<bool> callback)
        {
            _callbacksPool.ExecuteBool(() => _manager.AddNode(controller.HomeID, false), callback, 5*60);
        }

        public void AddNewSecureDevice(Controller controller, Action<bool> callback)
        {
            _callbacksPool.ExecuteBool(() => _manager.AddNode(controller.HomeID, true), callback, 5*60);
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
            _callbacksPool.ExecuteBool(() => _manager.TransferPrimaryRole(controller.HomeID), callback, 5*60);
        }

        public void CreateNewPrimary(Controller controller, Action<bool> callback)
        {
            _callbacksPool.ExecuteBool(() => _manager.CreateNewPrimary(controller.HomeID), callback, 5*60);
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

        private void ControllerStateHandle(ZWControllerState state)
        {
            bool? operationFailed = null;
            switch (state)
            {
                case ZWControllerState.Cancel:
                case ZWControllerState.Error:
                case ZWControllerState.Failed:
                case ZWControllerState.NodeFailed:
                    operationFailed = true;
                    break;
                case ZWControllerState.Completed:
                case ZWControllerState.NodeOK:
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
        }

        public void Initialize()
        {
            State = ZWaveManagerState.Initializing;
            var hasAnyControllers = LoadControllers();
            SetOptions();
            _manager = ZWManager.Instance;

            _manager.NotificationReceived += (s, e) =>
            {                
                var homeId = e.Notification.HomeId;
                var path =_manager.GetControllerPath(homeId);
                var controller = _controllers.FirstOrDefault(x => x.Path.Equals(path));
                var nodeId = e.Notification.NodeId;
                var node = _nodes.FirstOrDefault(x => x.Id.Equals(nodeId) && x.HomeId.Equals(homeId));
                var valueId = e.Notification.ValueId;
                var notificationType = e.Notification.Type;
                switch (notificationType)
                {
                    case ZWNotificationType.ControllerCommand:
                        {
                            ControllerStateHandle((ZWControllerState)e.Notification.Event);
                        }
                        break;
                    case ZWNotificationType.DriverRemoved:
                        {
                            _nodes.RemoveAll(x => x.HomeId.Equals(homeId));
                            _callbacksPool.Dequeue(true, nameof(RemoveController));
                        }
                        break;
                    case ZWNotificationType.DriverFailed:
                        {
                            controller.Failed = true;
                            if (_controllers.All(x => x.Failed))
                            {
                                State = ZWaveManagerState.Initialized;
                                ManagerInitializedCallbacksPool.ExecuteAll(this);
                            }
                            _callbacksPool.Dequeue(false,
                                nameof(AddController), 
                                nameof(RemoveController));
                        }
                        break;
                    case ZWNotificationType.DriverReady:
                        {
                            controller.Failed = false;
                            controller.HomeID = homeId;
                            _callbacksPool.Dequeue(
                                true,
                                nameof(AddController));
                        }
                        break;
                    case ZWNotificationType.AwakeNodesQueried:
                    case ZWNotificationType.AllNodesQueriedSomeDead:
                    case ZWNotificationType.AllNodesQueried:
                        {
                            _nodes.Where(x => !x.Initialized).All(x => x.Failed = true);
                            _manager.WriteConfig(homeId);
                            State = ZWaveManagerState.Initialized;
                            ManagerInitializedCallbacksPool.ExecuteAll(this);
                        }
                        break;
                    case ZWNotificationType.NodeAdded:
                    case ZWNotificationType.NodeNew:
                        {
                            node = new Node(nodeId, homeId, ZWManager.Instance);
                            _nodes.Add(node);
                            node.Controller = controller;
                            _manager.RequestNodeDynamic(node.HomeId, node.Id);
                            _manager.RequestAllConfigParams(node.HomeId, node.Id);

                            _callbacksPool.Dequeue(true, 
                                nameof(AddNewDevice),
                                nameof(AddNewSecureDevice));
                        }
                        break;
                    case ZWNotificationType.EssentialNodeQueriesComplete:
                    case ZWNotificationType.NodeQueriesComplete:
                        {
                            node.Initialized = true;
                        }
                        break;
                    case ZWNotificationType.NodeProtocolInfo:
                    case ZWNotificationType.NodeNaming:
                        {
                            node.Refresh();
                            node.Failed = false;
                        }
                        break;
                    case ZWNotificationType.NodeRemoved:
                        {
                            _nodes.Remove(node);
                            _callbacksPool.Dequeue(true,
                                nameof(RemoveDevice));
                        }
                        break;
                    case ZWNotificationType.ValueAdded:
                        {
                            if (!node.Values.Any(x => x.Id == valueId.Id)) //crutch
                            {
                                var nodeValue = new NodeValue(valueId, node);
                                if (valueId.Genre == ZWValueGenre.Config && valueId.Index < 256)
                                    node.RequestConfigParam((byte)valueId.Index); //crutch
                                node.Values.Add(nodeValue);
                                nodeValue.Refresh();
                                NodeValueChanged?.Invoke(this, new EventsArgs<NodeValue>(nodeValue));
                            }
                        }
                        break;
                    case ZWNotificationType.ValueRefreshed:
                        {
                            var nodeValue = node.Values.FirstOrDefault(x => x.Id.Equals(valueId));
                            nodeValue.Refresh();
                            NodeValueChanged?.Invoke(this, new EventsArgs<NodeValue>(nodeValue));
                        }
                        break;
                    case ZWNotificationType.ValueRemoved:
                        {
                            if (node != null)
                            {
                                var nodeValue = node.Values.FirstOrDefault(x => x.Id.Equals(valueId.Id));
                                node.Values.Remove(nodeValue);
                            }
                        }
                        break;
                    case ZWNotificationType.ValueChanged:
                        {
                            var nodeValue = node.Values.FirstOrDefault(x => x.Id.Equals(valueId.Id));
                            nodeValue.CurrentGroupIdx = e.Notification.GroupIndex;
                            nodeValue.InternalSet(Helper.GetValue(_manager, valueId, nodeValue.ZWValueType, nodeValue.PossibleValues));
                            NodeValueChanged?.Invoke(this, new EventsArgs<NodeValue>(nodeValue));
                        }
                        break;
                }
            };
            _manager.Initialize();
            foreach (var controller in _controllers)
            {
                var ctrl = controller;
                _callbacksPool.ExecuteBool(
                    () => _manager.AddDriver(ctrl.Path, ctrl.IsHID ? ZWControllerInterface.Hid : ZWControllerInterface.Serial),
                    (result) =>
                    {
                        if (!result)
                        {
                            ctrl.Failed = true;
                            if (_controllers.All(x => x.Failed) && State != ZWaveManagerState.Initialized)
                            {
                                State = ZWaveManagerState.Initialized;
                                ManagerInitializedCallbacksPool.ExecuteAll(this);
                            }
                        }
                        else
                            ctrl.Failed = false;
                    }, 
                    60,
                    "ControllerLoading" + controller.Path);
                _manager.TestNetwork(controller.HomeID, 1);
            }
            if ((!_controllers.Any() || _controllers.All(x => x.Failed)) && State != ZWaveManagerState.Initialized)
            {
                State = ZWaveManagerState.Initialized;
                ManagerInitializedCallbacksPool.ExecuteAll(this);
            }
        }

        public bool CancelOperation(Controller controller) => 
            _manager.CancelControllerCommand(controller.HomeID);

        private void SetOptions()
        {
            var options = ZWOptions.Instance;
            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "config");
            options.Initialize(path, path, string.Empty);
            options.AddOptionInt("SaveLogLevel", (int)ZWLogLevel.Info);
            options.AddOptionBool("AssumeAwake", true);
            options.AddOptionBool("SaveConfiguration", true);
            options.AddOptionBool("RefreshAllUserCodes", true);
            options.Lock();
        }

        private bool LoadControllers()
        {
            var dataManager = Singleton.Resolve<PluginsDataManagerBase>();
            try
            {
                if (dataManager.Has(_key))
                {
                    _controllers = dataManager.Get<List<Controller>>(_key);
                    return true;
                }
                else return false;
            }
            catch
            {
                return false;
            }
        }
        
        public event EventsHandler<NodeValue> NodeValueChanged;
    }
}