using System;
using System.Windows.Controls;
using System.Windows.Media;

namespace LazuriteUI.Windows.Main.Switches.SwitchSettings
{
    /// <summary>
    /// Логика взаимодействия для SwitchIconView.xaml
    /// </summary>
    public partial class SwitchIconView : Grid
    {
        public SwitchIconView()
        {
            InitializeComponent();
            MouseLeftButtonDown += (o, e) => {
                ((SwitchIconModel)DataContext).Apply();
                Selected?.Invoke();
            };
            var backgroundTemp = Background;
            MouseEnter += (o, e) => Background = Brushes.Transparent;
            MouseLeave += (o, e) => Background = backgroundTemp;
        }

        public Action Selected { get; set; }
    }
}
