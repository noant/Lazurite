using LazuriteUI.Windows.Controls;
using System;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace LazuriteUI.Windows.Main.Switches
{
    /// <summary>
    /// Логика взаимодействия для StatusViewSwitch.xaml
    /// </summary>
    public partial class StatusViewSwitch : UserControl
    {
        private Timer _timer;

        public StatusViewSwitch()
        {
            InitializeComponent();
        }

        public StatusViewSwitch(ScenarioModel scenarioModel): this()
        {
            DataContext = scenarioModel;

            BeginInit();

            spSearch.Visibility = scenarioModel.AcceptedValues.Length > 150 ? Visibility.Visible : Visibility.Collapsed;

            tbScenarioName.Text = scenarioModel.ScenarioName;
            ItemViewFast toSelect = null;
            foreach (var state in scenarioModel.AcceptedValues)
            {
                var itemView = new ItemViewFast();
                itemView.Text = state;
                itemView.Margin = new Thickness(0, 1, 0, 1);
                if (scenarioModel.ScenarioValue != null && scenarioModel.ScenarioValue.Equals(state))
                    toSelect = itemView;
                listItemsStates.Children.Add(itemView);
            }

            Loaded += (o, e) =>
            {
                if (toSelect != null)
                {
                    toSelect.Selected = true;
                    toSelect.Focus();
                }
            };

            listItemsStates.SelectionChanged += (o, e) =>
            {
                var selectedItem = listItemsStates.GetSelectedItems().FirstOrDefault() as ItemViewFast;
                if (selectedItem != null && selectedItem.Text != scenarioModel.ScenarioValue)
                {
                    scenarioModel.ScenarioValue = selectedItem.Text;
                    StateChanged?.Invoke(this, new RoutedEventArgs());
                }
            };

            tbSearch.TextChanged += (o, e) => {
                _timer?.Dispose();
                _timer = new Timer((s) => {
                    _timer = null;
                    Dispatcher.BeginInvoke(new Action(() => {
                        var text = tbSearch.Text.ToLowerInvariant().Trim();
                        foreach (ItemViewFast itemView in listItemsStates.Children)
                            itemView.Visibility =
                            string.IsNullOrEmpty(text) || itemView.Text.Contains(text) ?
                            Visibility.Visible :
                            Visibility.Collapsed;
                    }));
                }, null, 600, Timeout.Infinite);
            };

            EndInit();
        }

        public event Action<object, RoutedEventArgs> StateChanged; 
    }
}
