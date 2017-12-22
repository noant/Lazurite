using LazuriteUI.Icons;
using LazuriteUI.Windows.Controls;
using OpenZWrapper;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ZWavePluginUI
{
    /// <summary>
    /// Логика взаимодействия для ControllersManagerViiew.xaml
    /// </summary>
    public partial class ControllersManagerView : UserControl, ICanBlockUI
    {
        private ZWaveManager _manager;

        private Controller _selectedController;

        public Action<bool> BlockUI
        {
            get;
            set;
        }

        public ControllersManagerView()
        {
            InitializeComponent();
        }

        public void InitializeWith(ZWaveManager manager)
        {
            _manager = manager;
            if (_manager.State == ZWaveManagerState.None)
                _manager.Initialize();
            if (_manager.State == ZWaveManagerState.Initializing)
            {
                var messageView = new MessageView();
                BlockUI?.Invoke(true);
                var token = MessageView.ShowLoad("Инициализация менеджера контроллеров...", this.mainGrid);
                _manager.ManagerInitializedCallbacksPool.Add(new ManagerInitializedCallback() {
                    Callback = (sender, args) =>
                        this.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            token.Cancel();
                            RefreshControllersList();
                            BlockUI?.Invoke(false);
                        })),
                    RemoveAfterInvoke = true
                });
            }
            if (_manager.State == ZWaveManagerState.Initialized)
                RefreshControllersList();
        }

        private void RefreshControllersList()
        {
            controllersListView.Children.Clear();
            _selectedController = null;
            foreach (var controller in _manager.GetControllers())
                controllersListView.Children.Add(new ControllerView(controller, _manager));
            UpdateControls();
        }

        private void UpdateControls()
        {
            itemsViewControllerCommands.IsEnabled = _selectedController != null;
        }

        private void controllersListView_SelectionChanged(object sender, RoutedEventArgs e)
        {
            _selectedController = ((ControllerView)controllersListView.GetSelectedItems().FirstOrDefault()).Controller;
            UpdateControls();
        }

        private void itemViewAddNewDevice_Click(object sender, RoutedEventArgs e)
        {
            ProgressAction(
                "Добавление нового устройства... \r\nПереведите устройство в режим подключения и поднесите к контроллеру Z-Wave.",
                (callback) => _manager.AddNewDevice(_selectedController, callback), 
                needRefresh: false,
                cancel: _manager.SupportsCancellation(nameof(_manager.AddNewDevice)));
        }

        private void itemViewAddNewSecureDevice_Click(object sender, RoutedEventArgs e)
        {
            ProgressAction(
                "Добавление нового защищенного устройства... \r\nПереведите устройство в режим подключения и поднесите к контроллеру Z-Wave.",
                (callback) => _manager.AddNewSecureDevice(_selectedController, callback),
                needRefresh: false,
                cancel: _manager.SupportsCancellation(nameof(_manager.AddNewSecureDevice)));
        }

        private void itemViewRemoveDevice_Click(object sender, RoutedEventArgs e)
        {
            ProgressAction(
                "Удаление устройства... \r\nПереведите устройство в режим подключения и поднесите к контроллеру Z-Wave.",
                (callback) => _manager.RemoveDevice(_selectedController, callback),
                needRefresh: false,
                cancel: _manager.SupportsCancellation(nameof(_manager.RemoveDevice)));
        }

        private void itemViewHealNetwork_Click(object sender, RoutedEventArgs e)
        {
            ProgressAction(
                "Настройка сети...",
                (callback) => {
                    _manager.HealControllerNetwork(_selectedController);
                    callback(true);
                });
        }

        private void itemViewRefreshNetwork_Click(object sender, RoutedEventArgs e)
        {
            SelectNodeAnd((node) =>
                ProgressAction(
                    "Обновление устройства...",
                    (callback) => _manager.UpdateNetwork(_selectedController, node, callback),
                    needRefresh: false,
                    cancel: _manager.SupportsCancellation(nameof(_manager.UpdateNetwork))));
        }

        private void itemViewTransferPrimaryRole_Click(object sender, RoutedEventArgs e)
        {
            ProgressAction(
                "Передача роли основного контроллера...\r\nРасположите новый контроллер не далее 2-х метров от текущего.",
                (callback) => _manager.TransferPrimaryRole(_selectedController, callback),
                needRefresh: false,
                cancel: _manager.SupportsCancellation(nameof(_manager.TransferPrimaryRole)));
        }

        private void itemViewCreateNewPrimaryController_Click(object sender, RoutedEventArgs e)
        {
            ProgressAction(
                "Создание нового основного контроллера...\r\nРасположите новый контроллер не далее 2-х метров от текущего.",
                (callback) => _manager.CreateNewPrimary(_selectedController, callback),
                needRefresh: false,
                cancel: _manager.SupportsCancellation(nameof(_manager.CreateNewPrimary)));
        }

        private void itemViewSoftReset_Click(object sender, RoutedEventArgs e)
        {
            ProgressAction(
                "Мягкий сброс устройства...",
                (callback) => _manager.ResetController(_selectedController, callback),
                needRefresh: true,
                cancel: _manager.SupportsCancellation(nameof(_manager.ResetController)));
        }

        private void itemViewEraseAll_Click(object sender, RoutedEventArgs e)
        {
            ProgressAction(
                "Полный сброс устройства...",
                (callback) => _manager.EraseAll(_selectedController, callback),
                needRefresh: true,
                cancel: _manager.SupportsCancellation(nameof(_manager.EraseAll)));
        }

        private void itemViewRemoveController_Click(object sender, RoutedEventArgs e)
        {
            ProgressAction(
                "Удаление контроллера...",
                (callback) => _manager.RemoveController(_selectedController, callback),
                needRefresh: true,
                cancel: _manager.SupportsCancellation(nameof(_manager.RemoveController)));
        }

        private void itemViewReplaceFailedNode_Click(object sender, RoutedEventArgs e)
        {
            SelectNodeAnd((node) =>
                ProgressAction(
                    "Замена испорченного устройства...",
                    (callback) => _manager.ReplaceFailedNode(_selectedController, node, callback),
                    needRefresh: false,
                    cancel: _manager.SupportsCancellation(nameof(_manager.ReplaceFailedNode))));
        }

        private void itemViewRemoveFailedNode_Click(object sender, RoutedEventArgs e)
        {
            SelectNodeAnd((node) =>
                ProgressAction(
                    "Удаление испорченного устройства...",
                    (callback) => _manager.RemoveFailedNode(_selectedController, node, callback),
                    needRefresh: false,
                    cancel: _manager.SupportsCancellation(nameof(_manager.RemoveFailedNode))));
        }

        private void itemViewCheckNodeFailed_Click(object sender, RoutedEventArgs e)
        {
            SelectNodeAnd((node) =>
                ProgressAction(
                    "Проверка устройства...",
                    (callback) => _manager.CheckNodeFailed(_selectedController, node, callback),
                    needRefresh: false,
                    cancel: _manager.SupportsCancellation(nameof(_manager.CheckNodeFailed))));
        }

        private void itemViewUpdateNeighbors_Click(object sender, RoutedEventArgs e)
        {
            SelectNodeAnd((node) =>
                ProgressAction(
                    "Обновление списка узлов устройства...",
                    (callback) => _manager.UpdateNodeNeighborList(_selectedController, node, callback),
                    needRefresh: false,
                    cancel: _manager.SupportsCancellation(nameof(_manager.UpdateNodeNeighborList))));
        }

        private void itemViewRecieveConfiguration_Click(object sender, RoutedEventArgs e)
        {
            ProgressAction(
                "Передача конфигурации...\r\nРасположите контроллер не далее 2-х метров от нового устройства.",
                (callback) => _manager.RecieveConfiguration(_selectedController, callback),
                needRefresh: true,
                cancel: _manager.SupportsCancellation(nameof(_manager.RecieveConfiguration)));
        }

        private void itemViewAddNewController_Click(object sender, RoutedEventArgs e)
        {
            var action = new Action<Action<bool>>((callback) => 
                _manager.AddController(new Controller()
                {
                    Path = tbNewControllerName.Text,
                    IsHID = itemViewHID.Selected
                }, 
                callback));

            ProgressAction("Добавление контроллера '" + tbNewControllerName.Text + "'...", action, true, false);
        }

        private void tbNewControllerName_TextChanged(object sender, TextChangedEventArgs e)
        {
            itemViewAddNewController.IsEnabled = !string.IsNullOrWhiteSpace(tbNewControllerName.Text);
        }

        private void ProgressAction(string text, Action<Action<bool>> action, bool needRefresh=false, bool cancel=true)
        {
            BlockUI?.Invoke(true);
            var messageView = new MessageView();
            messageView.HeaderText = "Выполнение операции";
            messageView.ContentText = text;
            messageView.Icon = Icon.Hourglass;
            if (cancel)
                messageView.SetItems(new[] {
                    new MessageItemInfo("Отмена", (m) => _manager.CancelOperation(_selectedController), Icon.Cancel)
                });
            messageView.StartAnimateProgress();
            var callback = new Action<bool>((success)=> {
                this.Dispatcher.BeginInvoke(new Action(() => {
                    BlockUI?.Invoke(false);
                    messageView.StopAnimateProgress();
                    if (!success)
                    {
                        messageView.Icon = Icon.Cancel;
                        messageView.ContentText = "Операция не выполнена.";
                    }
                    else
                    {
                        messageView.Icon = Icon.Check;
                        messageView.ContentText = "Операция выполнена успешно!";
                        if (needRefresh)
                            RefreshControllersList();
                    }
                    messageView.SetItems(new[] {
                        new MessageItemInfo("OK", (m) => messageView.Close())
                    });
                }));
            });
            messageView.Show(mainGrid);
            action?.Invoke(callback);
        }

        private void SelectNodeAnd(Action<Node> callback)
        {
            var nodesView = new NodesListView();
            nodesView.InitializeWith(_manager, null, _selectedController);
            var dialogView = new DialogView(nodesView);
            nodesView.SelectedNodeChanged += (o, e) =>
            {
                dialogView.Close();
                callback(nodesView.SelectedNode);
            };
            dialogView.Show(mainGrid);
        }
    }
}
