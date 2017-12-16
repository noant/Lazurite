using LazuriteUI.Icons;
using LazuriteUI.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace LazuriteUI.Windows.Main.Switches.SwitchSettings
{
    /// <summary>
    /// Логика взаимодействия для SwitchIconSelect.xaml
    /// </summary>
    public partial class SwitchIconSelect : UserControl
    {
        private static string LastSearch;

        private static Dictionary<string, SwitchIconView> CachedControls = new Dictionary<string, SwitchIconView>();
        private static SwitchIconView Resolve(string icon)
        {
            if (CachedControls.ContainsKey(icon))
            {
                var control = CachedControls[icon];
                var parent = control.Parent as StackPanel;
                parent.Children.Remove(control);
                return control;
            }
            else
            {
                var control = new SwitchIconView();
                CachedControls.Add(icon, control);
                return control;
            }
        }

        public SwitchIconSelect(ScenarioModel scenarioModel, bool isSecondIcon)
        {
            InitializeComponent();
            var iconsControls = Enum.GetNames(typeof(Icon))
                .OrderBy(x => x)
                .Select(x => new SwitchIconModel(scenarioModel, isSecondIcon) { Icon = x })
                .Select(x =>
                {
                    var control = Resolve(x.Icon);
                    control.DataContext = x;
                    control.Margin = new Thickness(1, 1, 0, 0);
                    control.Selected = () => OkClick?.Invoke(this, new RoutedEventArgs());
                    return control;
                });

            int odd = 0;
            foreach (var control in iconsControls)
            {
                var sp = stackPanel1;
                if (odd == 1)
                    sp = stackPanel2;
                else if (odd == 2)
                    sp = stackPanel3;
                sp.Children.Add(control);
                odd++;
                if (odd == 3)
                    odd = 0;
            }

            this.tbSearch.Text = LastSearch;
        }

        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            var txt = tbSearch.Text.ToLower();

            LastSearch = txt;

            foreach (SwitchIconView iconView in stackPanel1.Children)
                if (IsIconNodeApply(txt, iconView))
                    iconView.Visibility = Visibility.Visible;
                else 
                    iconView.Visibility = Visibility.Collapsed;

            foreach (SwitchIconView iconView in stackPanel2.Children)
                if (IsIconNodeApply(txt, iconView))
                    iconView.Visibility = Visibility.Visible;
                else
                    iconView.Visibility = Visibility.Collapsed;

            foreach (SwitchIconView iconView in stackPanel3.Children)
                if (IsIconNodeApply(txt, iconView))
                    iconView.Visibility = Visibility.Visible;
                else
                    iconView.Visibility = Visibility.Collapsed;
        }

        private bool IsIconNodeApply(string str, SwitchIconView iconView)
        {
            if (string.IsNullOrEmpty(str))
                return true;
            return ((SwitchIconModel)iconView.DataContext).Icon.ToLower().Contains(str);
        }

        public event Action<object, RoutedEventArgs> OkClick;
    }
}
