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
            this.MouseLeftButtonDown += (o, e) => {
                ((SwitchIconModel)this.DataContext).Apply();
                Selected?.Invoke();
            };
            var backgroundTemp = this.Background;
            this.MouseEnter += (o, e) => this.Background = Brushes.Transparent;
            this.MouseLeave += (o, e) => this.Background = backgroundTemp;
        }

        public Action Selected { get; set; }
    }
}
