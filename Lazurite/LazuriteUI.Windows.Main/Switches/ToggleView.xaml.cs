using Lazurite.MainDomain;
using System.Windows.Controls;

namespace LazuriteUI.Windows.Main.Switches
{
    /// <summary>
    /// Логика взаимодействия для ToggleView.xaml
    /// </summary>
    public partial class ToggleView : UserControl
    {
        public ToggleView()
        {
            InitializeComponent();
        }

        public ToggleView(ScenarioBase scenario): this()
        {
            var model = new ScenarioModel(scenario);
            DataContext = model;
            Unloaded += (o, e) => model.Dispose();
        }
    }
}
