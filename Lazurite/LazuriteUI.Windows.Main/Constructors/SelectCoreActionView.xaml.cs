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

namespace LazuriteUI.Windows.Main.Constructors
{
    /// <summary>
    /// Логика взаимодействия для SelectCoreActionView.xaml
    /// </summary>
    public partial class SelectCoreActionView : UserControl
    {
        public SelectCoreActionView()
        {
            InitializeComponent();
            listItems.SelectionChanged += (o, e) =>
            {
                var selectedItem = listItems.SelectedItem;
                if (selectedItem!=null)
                {
                    Selected?.Invoke(((ItemView)selectedItem).Tag as Type);
                }
            };
        }

        public event Action<Type> Selected;

        public static void Show(Action<Type> callback)
        {
            var control = new SelectCoreActionView();
            var dialog = new DialogView(control);
            control.Selected += (type) =>
            {
                callback(type);
                dialog.Close();
            };
            dialog.ShowUnderCursor = true;
            dialog.Show();
        }
    }
}
