using LazuriteUI.Windows.Controls;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace LazuriteUI.Windows.Main.Switches
{
    /// <summary>
    /// Логика взаимодействия для StatusViewSwitch.xaml
    /// </summary>
    public partial class StatusViewSwitch : UserControl
    {
        public StatusViewSwitch()
        {
            InitializeComponent();
        }

        public StatusViewSwitch(ScenarioModel scenarioModel): this()
        {
            this.tbScenarioName.Text = scenarioModel.ScenarioName;
            ItemView toSelect = null;
            foreach (var state in scenarioModel.AcceptedValues)
            {
                var itemView = new ItemView();
                itemView.Icon = Icons.Icon.NavigateNext;
                itemView.Content = state;
                itemView.Margin = new Thickness(0, 0, 0, 1);
                if (scenarioModel.ScenarioValue != null && scenarioModel.ScenarioValue.Equals(state))
                    toSelect = itemView;
                listItemsStates.Children.Add(itemView);
            }

            this.Loaded += (o, e) =>
            {
                if (toSelect != null)
                {
                    toSelect.Selected = true;
                    toSelect.Focus();
                }
            };

            listItemsStates.SelectionChanged += (o, e) =>
            {
                var selectedItem = listItemsStates.GetSelectedItems().FirstOrDefault() as ItemView;
                if (selectedItem != null && selectedItem.Content.ToString() != scenarioModel.ScenarioValue)
                {
                    scenarioModel.ScenarioValue = selectedItem.Content.ToString();
                    StateChanged?.Invoke(this, new RoutedEventArgs());
                }
            };
        }

        public event Action<object, RoutedEventArgs> StateChanged; 
    }
}
