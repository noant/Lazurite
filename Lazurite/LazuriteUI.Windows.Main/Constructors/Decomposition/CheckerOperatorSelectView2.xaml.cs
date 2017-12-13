using LazuriteUI.Windows.Controls;
using System;
using System.Linq;
using System.Windows.Controls;

namespace LazuriteUI.Windows.Main.Constructors.Decomposition
{
    /// <summary>
    /// Логика взаимодействия для CheckerOperatorSelectView.xaml
    /// </summary>
    public partial class CheckerOperatorSelectView2 : UserControl
    {
        public CheckerOperatorSelectView2(bool not)
        {
            InitializeComponent();

            if (not)
                listItems.GetItems().Last().Selected = true;
            else listItems.GetItems().First().Selected = true;

            listItems.SelectionChanged += (o, e) => {
                Selected?.Invoke(listItems.GetItems().Last().Selected);
            };
        }

        public event Action<bool> Selected;

        public static void Show(Action<bool> callback, bool selectedValue)
        {
            var control = new CheckerOperatorSelectView2(selectedValue);
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
