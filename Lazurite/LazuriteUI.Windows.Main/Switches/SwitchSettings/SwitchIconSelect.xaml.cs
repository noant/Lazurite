using LazuriteUI.Icons;
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

namespace LazuriteUI.Windows.Main.Switches.SwitchSettings
{
    /// <summary>
    /// Логика взаимодействия для SwitchIconSelect.xaml
    /// </summary>
    public partial class SwitchIconSelect : UserControl
    {
        public SwitchIconSelect(ScenarioModel scenarioModel, bool isSecondIcon)
        {
            InitializeComponent();

            var iconsControls = Enum.GetNames(typeof(Icon))
                .OrderBy(x=>x)
                .Select(x => new SwitchIconModel(scenarioModel, isSecondIcon) { Icon = x } )
                .Select(x => new SwitchIconView() { DataContext = x });

            foreach (var control in iconsControls)
                stackPanel.Children.Add(control);
        }

        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            var txt = tbSearch.Text.ToLower();
            foreach (SwitchIconView iconView in stackPanel.Children)
            {
                if (IsIconNodeApply(txt, iconView))
                    iconView.Visibility = Visibility.Visible;
                else 
                    iconView.Visibility = Visibility.Collapsed;
            }
        }

        private bool IsIconNodeApply(string str, SwitchIconView iconView)
        {
            if (string.IsNullOrEmpty(str))
                return true;
            return ((SwitchIconModel)iconView.DataContext).Icon.ToLower().Contains(str);
        }

        private void ItemView_Click(object sender, RoutedEventArgs e)
        {
            OkClick?.Invoke(this, new RoutedEventArgs());
        }

        public event Action<object, RoutedEventArgs> OkClick;
    }
}
