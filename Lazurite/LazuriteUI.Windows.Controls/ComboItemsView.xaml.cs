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

namespace LazuriteUI.Windows.Controls
{
    /// <summary>
    /// Логика взаимодействия для ComboItemsView.xaml
    /// </summary>
    public partial class ComboItemsView : UserControl
    {
        public ComboItemsView()
        {
            InitializeComponent();
        }

        private ComboItemsViewInfo _info;

        public ComboItemsViewInfo Info {
            get => _info;
            set
            {
                _info = value;
                Refresh();
                InfoChanged?.Invoke(this, Info);
            }
        }
        
        private void Refresh()
        {
            if (Info.SelectedObjects?.Length > 1)
            {
                var strs = Info.SelectedObjects.Select(x => Info.GetCaption(x)).Aggregate((x1, x2) => x1 + ", " + x2);
                mainItem.Content = strs;
                mainItem.Icon = Icons.Icon._None;
            }
            else if (Info.SelectedObjects?.Length == 1)
            {
                var item = Info.SelectedObjects.FirstOrDefault();
                mainItem.Content = Info.GetCaption(item);
                mainItem.Icon = Info.GetIcon != null ? Info.GetIcon(item) : Icons.Icon._None;
            }
            else
            {
                mainItem.Content = "Не выбрано";
                mainItem.Icon = Icons.Icon._None;
            }
        }

        private void btSelect_Click(object sender, RoutedEventArgs e)
        {
            DialogView dialog = null;
            var listView = new ComboBoxItemsView_List(
                Info, 
                (info) => {
                    dialog.Close();
                    Info = info;
                });
            dialog = new DialogView(listView);
            dialog.Show(Info.MainPanel);
        }

        public event EventHandler<ComboItemsViewInfo> InfoChanged;
    }
}
