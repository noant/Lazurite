using LazuriteUI.Windows.Controls;
using System;
using System.Windows.Controls;

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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
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
