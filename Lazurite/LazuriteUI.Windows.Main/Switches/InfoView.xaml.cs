using Lazurite.MainDomain;
using System.Windows.Controls;

namespace LazuriteUI.Windows.Main.Switches
{
    /// <summary>
    /// Логика взаимодействия для ToggleView.xaml
    /// </summary>
    public partial class InfoView : UserControl
    {
        public InfoView()
        {
            InitializeComponent();
        }

        public InfoView(ScenarioBase scenario, UserVisualSettings visualSettings) : this()
        {
            var model = new ScenarioModel(scenario, visualSettings);
            DataContext = model;

            itemView.Click += (o, e) => {
                InfoViewSwitch.Show((newVal) => model.ScenarioValue = newVal);
            };
        }
    }
}
