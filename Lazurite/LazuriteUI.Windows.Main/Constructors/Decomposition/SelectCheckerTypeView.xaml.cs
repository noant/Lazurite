using LazuriteUI.Windows.Controls;
using System;
using System.Linq;
using System.Windows.Controls;

namespace LazuriteUI.Windows.Main.Constructors.Decomposition
{
    /// <summary>
    /// Логика взаимодействия для CheckerOperatorSelectView.xaml
    /// </summary>
    public partial class SelectCheckerTypeView : UserControl
    {
        public SelectCheckerTypeView()
        {
            InitializeComponent();
            
            listItems.SelectionChanged += (o, e) => {
                Selected?.Invoke(listItems.GetItems().Last().Selected);
            };
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event Action<bool> Selected;

        public static void Show(Action<bool> callback)
        {
            var control = new SelectCheckerTypeView();
            var dialog = new DialogView(control);
            dialog.ShowUnderCursor = true;
            control.Selected += (result) => {
                callback?.Invoke(result);
                dialog.Close();
            };
            dialog.Show();
        }
    }
}
