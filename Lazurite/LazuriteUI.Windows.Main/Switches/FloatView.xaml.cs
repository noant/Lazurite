using Lazurite.ActionsDomain;
using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.MainDomain;
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

namespace LazuriteUI.Windows.Main.Switches
{
    /// <summary>
    /// Логика взаимодействия для ToggleView.xaml
    /// </summary>
    public partial class FloatView : UserControl
    {
        public FloatView()
        {
            InitializeComponent();
        }

        public FloatView(ScenarioBase scenario, UserVisualSettings visualSettings): this()
        {
            var model = new ScenarioModel(scenario, visualSettings);
            this.DataContext = model;
            //sometimes binding works incorrectly
            //this.scaleView.Value = double.Parse(model.ScenarioValue);
        }

        private void itemView_Click(object sender, RoutedEventArgs e)
        {
            var floatSwitch = new FloatViewSwitch((ScenarioModel)this.DataContext);
            var dialog = new DialogView(floatSwitch);
            dialog.Show();
        }
    }
}
