using Lazurite.Windows.Logging;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

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
