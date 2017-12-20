using Lazurite.IOC;
using Lazurite.Windows.Logging;
using Lazurite.Windows.Modules;
using LazuriteUI.Icons;
using LazuriteUI.Windows.Controls;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace LazuriteUI.Windows.Main.PluginsViews
{
    /// <summary>
    /// Логика взаимодействия для PluginsView.xaml
    /// </summary>
    [System.ComponentModel.DisplayName("Управление плагинами")]
    [Icons.LazuriteIcon(Icons.Icon.Brick)]
    public partial class PluginsView : UserControl, IInitializable
    {
        private PluginsManager _manager = Singleton.Resolve<PluginsManager>();
        private WarningHandlerBase _warningHandler = Singleton.Resolve<WarningHandlerBase>();

        public PluginsView()
        {
            InitializeComponent();
            
            itemsView.SelectionChanged += (o, e) => {
                spPluginTypes.Children.Clear();
                if (itemsView.GetSelectedItems().Any())
                {
                    var types = ((ItemView)itemsView.SelectedItem).Tag as Type[];
                    foreach (var type in types)
                    {
                        var pluginActionView = new PluginActionView(type);
                        spPluginTypes.Children.Add(pluginActionView);
                    }
                    btRemovePlugin.Visibility = Visibility.Visible;
                }
                else
                {
                    btRemovePlugin.Visibility = Visibility.Collapsed;
                    spPluginTypes.Children.Clear();
                }
            };
        }
        
        public void Refresh()
        {
            itemsView.Children.Clear();
            foreach (var plugin in _manager.GetPlugins())
            {
                var itemView = new ItemView();
                itemView.Icon = Icons.Icon.Layer;
                itemView.Content = plugin.Name;
                itemView.Margin = new Thickness(1);
                itemView.Tag = _manager
                    .GetPluginsTypesInfos()
                    .Where(x => x.Plugin.Equals(plugin))
                    .Select(x => x.Type)
                    .ToArray();
                itemsView.Children.Add(itemView);
            }
            if (itemsView.GetItems().Any())
                itemsView.GetItems().First().Selected = true;
            else
            {
                btRemovePlugin.Visibility = Visibility.Collapsed;
                spPluginTypes.Children.Clear();
            }
        }

        private void btAddPlugin_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.OpenFileDialog();
            dialog.Filter = string.Format("Lazurite plugin file (*{0})|*{0}", PluginsManager.PluginFileExtension);
            dialog.Multiselect = false;
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var canAddPluginResult = _manager.CanAddPlugin(dialog.FileName);
                if (!canAddPluginResult.CanAdd)
                {
                    _warningHandler.WarnFormat("Невозможно добавить плагин \r\n[{0}];\r\nПричина: {1}", dialog.FileName, canAddPluginResult.Message);
                }
                else
                {
                    try
                    {
                        _manager.AddPlugin(dialog.FileName);
                        MessageView.ShowMessage("Плагин добавлен!", "Добавление плагина", Icons.Icon.Check, Window.GetWindow(this).Content as Panel);
                        Refresh();
                    }
                    catch (Exception exception)
                    {
                        _warningHandler.ErrorFormat(exception, "Невозможно добавить плагин \r\n[{0}]", dialog.FileName);
                    }
                }
            }
        }

        public PluginInfo SelectedPlugin
        {
            get
            {
                var pluginName = (itemsView.GetSelectedItems().FirstOrDefault() as ItemView)?.Content.ToString();
                return _manager.GetPlugins().FirstOrDefault(x => x.Name.Equals(pluginName));
            }
        }

        private void btRemovePlugin_Click(object sender, RoutedEventArgs e)
        {
            MessageView.ShowYesNo("Вы уверены что хотите удалить плагин [" + SelectedPlugin.Name + "]?", "Удаленеи плагина", Icons.Icon.Delete,
                (result) => {
                    if (result)
                    {
                        var canRemoveResult = _manager.CanRemovePlugin(SelectedPlugin.Name);
                        if (!canRemoveResult.CanRemove)
                        {
                            MessageView.ShowMessage(string.Format("Невозможно удалить плагин \r\n[{0}];\r\nПричина: {1}", SelectedPlugin.Name, canRemoveResult.Message), "Внимание!", Icon.Warning);
                        }
                        else
                        {
                            try
                            {
                                _manager.RemovePlugin(SelectedPlugin.Name);
                                Refresh();
                                MessageView.ShowMessage("Плагин удален. Повторно добавить этот плагин можно будет при следующем запуске программы.", "Удаление плагина", Icon.Warning);
                            }
                            catch (Exception exception)
                            {
                                _warningHandler.WarnFormat(exception, "Невозможно удалить плагин \r\n[{0}]", SelectedPlugin.Name);
                            }
                        }
                    }
                });
        }

        private void updatePlugin_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.OpenFileDialog();
            dialog.Filter = string.Format("Lazurite plugin file (*{0})|*{0}", PluginsManager.PluginFileExtension);
            dialog.Multiselect = false;
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    _manager.UpdatePlugin(dialog.FileName);
                    MessageView.ShowMessage("Плагин обновлен! Изменения вступят в силу после перезапуска приложения.", "Обновление плагина", Icons.Icon.Check, Window.GetWindow(this).Content as Panel);
                    Refresh();
                }
                catch (Exception exception)
                {
                    _warningHandler.ErrorFormat(exception, "Невозможно обновить плагин \r\n[{0}]", dialog.FileName);
                }
            }
        }

        public void Initialize()
        {
            Refresh();
        }
    }
}