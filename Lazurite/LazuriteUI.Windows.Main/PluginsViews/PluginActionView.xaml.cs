using System;
using System.Windows.Controls;

namespace LazuriteUI.Windows.Main.PluginsViews
{
    /// <summary>
    /// Логика взаимодействия для PluginActionView.xaml
    /// </summary>
    public partial class PluginActionView : UserControl
    {
        public PluginActionView(Type actionType)
        {
            InitializeComponent();
            this.DataContext = new PluginActionViewModel(actionType);
        }
    }
}
