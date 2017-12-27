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
    /// Логика взаимодействия для NodesMainView.xaml
    /// </summary>
    public partial class NodesMainView : UserControl
    {
        public NodesMainView()
        {
            InitializeComponent();
            nodesListView.SelectionChanged += (o, e) => {
                nodesValuesListView.RefreshWith(nodesListView.SelectedNode);
                UpdateControls();
            };
            genreSelectView.SelectedGenreChanged += (o, e) => {
                nodesValuesListView.SelectedGenre = genreSelectView.SelectedGenre;
                UpdateControls();
            };
            nodesValuesListView.SelectionChanged += (o, e) => {
                SelectedNodeValue = nodesValuesListView.SelectedNodeValue;
                descriptionView.RefreshWith(SelectedNodeValue);
                UpdateControls();
            };
            btControllers.Click += (o, e) => {
                ControllersManagerView.Show(_manager, this.gridExternalContent, Refresh);
            };
            
            btRefresh.Click += (o, e) => Refresh();
            UpdateControls();
        }

        private ZWaveManager _manager;

        public NodeValue SelectedNodeValue { get; private set; }
        
        public void RefreshWith(ZWaveManager manager, NodeValue selectedValue = null)
        {
            _manager = manager;
            SelectedNodeValue = selectedValue;
            if (_manager.State == ZWaveManagerState.None)
                _manager.Initialize();
            if (_manager.State == ZWaveManagerState.Initializing)
            {
                var token = MessageView.ShowLoad("Инициализация контроллеров...", this.gridExternalContent);
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
                ControllersManagerView.Show(_manager, this.gridExternalContent, Refresh);
        }

        private void Refresh()
        {
            nodesListView.RefreshWith(_manager.GetNodes());
            nodesListView.SelectedNode = SelectedNodeValue?.Node;
            nodesValuesListView.SelectedNodeValue = SelectedNodeValue;
        }

        private void UpdateControls() => btApply.IsEnabled = nodesValuesListView.SelectedItem != null;
    }
}
