using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.IOC;
using Lazurite.MainDomain;
using Lazurite.Windows.Modules;
using LazuriteUI.Windows.Controls;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LazuriteUI.Windows.Main.Constructors
{
    /// <summary>
    /// Логика взаимодействия для SelectScenarioView.xaml
    /// </summary>
    public partial class SelectScenarioView : UserControl
    {
        private static readonly ScenariosRepositoryBase ScenariosRepository = Singleton.Resolve<ScenariosRepositoryBase>();
        private static readonly UsersRepositoryBase UsersRepository = Singleton.Resolve<UsersRepositoryBase>();
        
        public ScenarioBase SelectedScenario { get; private set; }

        public SelectScenarioView()
        {
            InitializeComponent();
            Loaded += (sender, e) => MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
        }

        public void Initialize(Type valueType = null, ActionInstanceSide side = ActionInstanceSide.Both, string selectedScenarioId = "")
        {
            var scenarioActionSource = new ScenarioActionSource(
                UsersRepository.SystemUser, 
                ScenarioStartupSource.OtherScenario, 
                side == ActionInstanceSide.OnlyRight ? ScenarioAction.ViewValue : ScenarioAction.Execute);
            var scenarios = ScenariosRepository.Scenarios.Where(x => valueType == null || x.ValueType.GetType().Equals(valueType));
            if (side == ActionInstanceSide.OnlyRight)
                scenarios = scenarios.Where(x => !(x.ValueType is ButtonValueType) && x.IsAccessAvailable(scenarioActionSource));
            else
                scenarios = scenarios.Where(x => x.IsAccessAvailable(scenarioActionSource));

            foreach (var scenario in scenarios)
            {
                var itemView = new ItemView();
                itemView.Content = scenario.Name;
                if (scenario.Id.Equals(selectedScenarioId))
                    Loaded += (o, e) => itemView.Selected = true;
                itemView.Icon = Icons.Icon.ChevronRight;
                itemView.Tag = scenario;
                itemView.Margin = new Thickness(2);
                itemView.Click += (o, e) => 
                {
                    SelectedScenario = itemView.Tag as ScenarioBase;
                    SelectionChanged?.Invoke(this);
                };
                itemsView.Children.Add(itemView);
            }

            if (itemsView.Children.Count > 0)
            {
                tbScensNotExist.Visibility = Visibility.Collapsed;
                MinHeight = 0;
            }
        }
        
        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            var txt = tbSearch.Text.ToUpper().Trim();
            foreach (ItemView item in itemsView.GetItems())
            {
                if (string.IsNullOrEmpty(txt) || item.Content.ToString().ToUpper().Contains(txt))
                    item.Visibility = Visibility.Visible;
                else item.Visibility = Visibility.Collapsed;
            }
        }

        public Action<SelectScenarioView> SelectionChanged;

        public static void Show(Action<ScenarioBase> selectedCallback, Type valueType = null, ActionInstanceSide side = ActionInstanceSide.Both, string selectedScenarioId = "")
        {
            var control = new SelectScenarioView();
            var dialogView = new DialogView(control);
            control.Initialize(valueType, side, selectedScenarioId);
            control.SelectionChanged += (ctrl) =>
            {
                selectedCallback?.Invoke(control.SelectedScenario);
                dialogView.Close();
            };
            dialogView.Show();
        }
    }
}