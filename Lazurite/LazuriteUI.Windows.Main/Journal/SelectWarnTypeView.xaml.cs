using Lazurite.Windows.Logging;
using LazuriteUI.Windows.Controls;
using System;
using System.Windows;
using System.Windows.Controls;

namespace LazuriteUI.Windows.Main.Journal
{
    /// <summary>
    /// Логика взаимодействия для SelectWarnTypeView.xaml
    /// </summary>
    public partial class SelectWarnTypeView : UserControl
    {
        public SelectWarnTypeView()
        {
            InitializeComponent();
        }
        
        private void ListItemsView_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (listView.SelectedItem != null)
            {
                Selected?.Invoke((WarnType)Enum.Parse(typeof(WarnType), ((ItemView)listView.SelectedItem).Content.ToString()));
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event Action<WarnType> Selected;

        public static void Show(Action<WarnType> callback)
        {
            var control = new SelectWarnTypeView();
            var dialog = new DialogView(control);
            control.Selected += (warnType) => {
                dialog.Close();
                callback?.Invoke(warnType);
            };
            dialog.ShowUnderCursor = true;
            dialog.Show();
        }
    }
}
