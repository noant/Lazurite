using LazuriteUI.Windows.Controls;
using OpenZWrapper;
using System;
using System.Linq;
using System.Windows;

namespace ZWavePluginUI
{
    /// <summary>
    /// Логика взаимодействия для ZWaveSelector.xaml
    /// </summary>
    public partial class ZWaveSelectionWindow : Window
    {
        private ZWaveManager _manager;
        private FrameworkElement _primary;
        public ZWaveSelectionWindow(ZWaveManager manager)
        {
            InitializeComponent();
            _manager = manager;

            this.Loaded += (o, e) => {
                BlockButtons(true);
                var messageView = new MessageView();
                var token = MessageView.ShowLoad("Инициализация контроллеров...", this.crutchGrid);
                _manager.ManagerInitializedCallbacksPool.Add(new ManagerInitializedCallback() {
                    Callback = (o1, e1) => this.Dispatcher.BeginInvoke(new Action(() => 
                    {
                        messageView.Close();
                        this.itemViewPrimary.Selected = true;
                        BlockButtons(false);
                    })),
                    RemoveAfterInvoke = true
                });
                if (manager.State == ZWaveManagerState.None)
                    manager.Initialize();
                else if (manager.State == ZWaveManagerState.Initialized)
                {
                    messageView.Close();
                    this.itemViewPrimary.Selected = true;
                    BlockButtons(false);
                }
            };
        }

        public void SetPrimaryControl(FrameworkElement primary)
        {
            _primary = primary;
            if (_primary is IRefreshable)
            {
                ((IRefreshable)_primary).IsDataAllowed = (allowed) => okItem.IsEnabled = allowed;
                ((IRefreshable)_primary).NeedClose = () => this.Close();
            }
        }

        private void ViewPrimary()
        {
            mainContent.Children.Clear();
            mainContent.Children.Add(_primary);
            if (_primary is IRefreshable)
                ((IRefreshable)_primary).Refresh();
        }
        
        private void ListItemsView_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (listViewModes.GetSelectedItems().FirstOrDefault() == itemViewPrimary)
                ViewPrimary();
            else if (listViewModes.GetSelectedItems().FirstOrDefault() == itemViewController)
            {
                mainContent.Children.Clear();
                var managerView = new ControllersManagerView();
                mainContent.Children.Add(managerView);
                managerView.BlockUI = (block) => BlockButtons(block);
                managerView.InitializeWith(_manager);
            }
        }

        private void BlockButtons(bool block)
        {
            this.cancelItem.IsEnabled =
                this.itemViewPrimary.IsEnabled =
                this.itemViewController.IsEnabled =
                !block;
        }

        private void cancelItem_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void okItem_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}