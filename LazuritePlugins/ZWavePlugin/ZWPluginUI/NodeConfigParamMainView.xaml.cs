using LazuriteUI.Windows.Controls;
using OpenZWrapper;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ZWPluginUI
{
    /// <summary>
    /// Логика взаимодействия для NodeConfigParamMainView.xaml
    /// </summary>
    public partial class NodeConfigParamMainView : UserControl
    {
        public NodeConfigParamMainView()
        {
            InitializeComponent();
            nodesListView.SelectionChanged += (o, e) =>
            {
                SelectedNode = nodesListView.SelectedNode;
                UpdateControls();
            };
            btControllers.Click += (o, e) => ControllersManagerView.Show(_manager, gridExternalContent, Refresh);
            btRefresh.Click += (o, e) => Refresh();
            tbId.Validation = EntryViewValidation.IntValidation();
            UpdateControls();
        }

        private ZWaveManager _manager;
        public Node SelectedNode { get; private set; }
        public byte SelectedParamId => byte.Parse(tbId.Text);
        
        public void RefreshWith(ZWaveManager manager, Node node = null, byte paramId = 0)
        {
            _manager = manager;
            SelectedNode = node;
            if (_manager.State == ZWaveManagerState.None)
                _manager.Initialize();
            if (_manager.State == ZWaveManagerState.Initializing)
            {
                var token = MessageView.ShowLoad("Инициализация контроллеров...", gridExternalContent);
                _manager.ManagerInitializedCallbacksPool.Add(new ManagerInitializedCallback()
                {
                    RemoveAfterInvoke = true,
                    Callback = (s, args) => {
                        token.Cancel();
                        if (args.Successful)
                            Refresh();
                        else
                            MessageView.ShowMessage("Ошибка инициализации контроллеров.", "Ошибка", LazuriteUI.Icons.Icon.Cancel);
                    }
                });
            }
            if (_manager.State == ZWaveManagerState.Initialized)
                Refresh();
            if(!_manager.GetControllers().Any())
                ControllersManagerView.Show(_manager, gridExternalContent, Refresh);

            tbId.Text = paramId.ToString();
        }

        private void Refresh()
        {
            nodesListView.RefreshWith(_manager.GetNodes());
            nodesListView.SelectedNode = SelectedNode;
        }

        private void UpdateControls() => ButtonApply.IsEnabled = nodesListView.SelectedItem != null && tbId.Text.Length > 0;
    }
}
