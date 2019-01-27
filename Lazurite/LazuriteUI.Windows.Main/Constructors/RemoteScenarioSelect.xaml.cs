using Lazurite.MainDomain;
using LazuriteUI.Icons;
using LazuriteUI.Windows.Controls;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event Action<ScenarioInfo> ScenarioInfoSelected;
    }
}
