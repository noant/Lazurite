using LazuriteUI.Windows.Controls;
using System;
using System.Linq;
using System.Windows.Controls;

namespace LazuriteUI.Windows.Main.Constructors.Decomposition
{
    /// <summary>
    /// Логика взаимодействия для CheckerOperatorSelectView.xaml
    /// </summary>
    public partial class CheckerOperatorSelectView : UserControl
    {
        public CheckerOperatorSelectView(bool orSelected)
        {
            InitializeComponent();

            if (orSelected)
                listItems.GetItems().Last().Selected = true;
            else listItems.GetItems().First().Selected = true;

            listItems.SelectionChanged += (o, e) => {
                Selected?.Invoke(listItems.GetItems().Last().Selected);
            };
        }

        public event Action<bool> Selected;

        public static void Show(Action<bool> callback, bool selectedValue)
        {
            var control = new CheckerOperatorSelectView(selectedValue);
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
