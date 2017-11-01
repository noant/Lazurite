using OpenZWrapper;
using LazuriteUI.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
                var messageView = new MessageView();
                messageView.HeaderText = "Пожалуйста, подождите...";
                messageView.ContentText = "Инициализация контроллеров...";
                messageView.Icon = LazuriteUI.Icons.Icon.Hourglass;
                messageView.StartAnimateProgress();
                messageView.Show(this.crutchGrid);
                _manager.ManagerInitializedCallbacksPool.Add(new ManagerInitializedCallback() {
                    Callback = (o1, e1) => this.Dispatcher.BeginInvoke(new Action(() => 
                    {
                        messageView.Close();
                        this.itemViewPrimary.Selected = true;
                    })),
                    RemoveAfterInvoke = true
                });
                if (manager.State == ZWaveManagerState.None)
                    manager.Initialize();
                else if (manager.State == ZWaveManagerState.Initialized)
                {
                    messageView.Close();
                    this.itemViewPrimary.Selected = true;
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
                managerView.InitializeWith(_manager);
                mainContent.Children.Add(managerView);
            }
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