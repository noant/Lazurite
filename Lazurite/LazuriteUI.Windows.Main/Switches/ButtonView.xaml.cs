using Lazurite.MainDomain;
using System.Windows.Controls;

namespace LazuriteUI.Windows.Main.Switches
{
    /// <summary>
    /// Логика взаимодействия для ToggleView.xaml
    /// </summary>
    public partial class ButtonView : UserControl
    {
        public ButtonView()
        {
            InitializeComponent();
        }

        public ButtonView(ScenarioBase scenario): this()
        {
            DataContext = new ScenarioModel(scenario);
            itemView.Click += (o, e) =>
                ((ScenarioModel)DataContext).ScenarioValue = string.Empty;
        }
    }
}
