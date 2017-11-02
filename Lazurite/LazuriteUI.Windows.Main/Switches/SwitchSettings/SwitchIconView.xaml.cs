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
