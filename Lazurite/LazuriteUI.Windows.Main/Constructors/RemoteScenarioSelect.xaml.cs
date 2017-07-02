using Lazurite.MainDomain;
using LazuriteUI.Icons;
using LazuriteUI.Windows.Controls;
using LazuriteUI.Windows.Main.Switches;
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

namespace LazuriteUI.Windows.Main.Constructors
{
    /// <summary>
    /// Логика взаимодействия для RemoteScenarioSelect.xaml
    /// </summary>
    public partial class RemoteScenarioSelect : UserControl
    {
        public RemoteScenarioSelect(ScenarioInfo[] scenariosInfos, string selectedScenario)
        {
            InitializeComponent();

            foreach (var info in scenariosInfos)
            {
                var itemView = new ItemView();
                itemView.Content = info.Name;
                itemView.Icon = Icon.ChevronRight;
                itemView.Tag = info;
                itemView.Margin = new Thickness(2);
                if (info.ScenarioId.Equals(selectedScenario))
                    itemView.Selected = true;
                listItems.Children.Add(itemView);
            }

            listItems.SelectionChanged += (o, e) =>
            {
                if (listItems.GetSelectedItems().Any())
                    ScenarioInfoSelected?.Invoke(((ItemView)listItems.SelectedItem).Tag as ScenarioInfo);
            };
        }

        public event Action<ScenarioInfo> ScenarioInfoSelected;
    }
}
