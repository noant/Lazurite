using Lazurite.CoreActions.CheckerLogicalOperators;
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
using Lazurite.CoreActions;
using Lazurite.MainDomain;
using LazuriteUI.Windows.Controls;

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
            control.Selected += (result) => {
                callback?.Invoke(result);
                dialog.Close();
            };
            dialog.Show();
        }
    }
}
