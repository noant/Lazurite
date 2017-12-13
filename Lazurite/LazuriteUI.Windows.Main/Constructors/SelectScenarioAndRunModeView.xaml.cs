using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.CoreActions.CoreActions;
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
    public partial class SelectScenarioAndRunModeView : UserControl
    {
        private ScenariosRepositoryBase _repository = Singleton.Resolve<ScenariosRepositoryBase>();
        
        public ScenarioBase SelectedScenario { get; private set; }
        public RunExistingScenarioMode SelectedMode { get; private set; }

        public SelectScenarioAndRunModeView()
        {
            InitializeComponent();
            Loaded += (sender, e) => MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
            Loaded += (sender, e) => {
                if (SelectedMode == RunExistingScenarioMode.Asynchronously)
                    btModeAsyncPar.Selected = true;
                else if (SelectedMode == RunExistingScenarioMode.MainExecutionContext)
                    btModeAsync.Selected = true;
                else if (SelectedMode == RunExistingScenarioMode.Synchronously)
                    btModeSync.Selected = true;
            };

            btModeAsync.Click += (o, e) => this.SelectedMode = RunExistingScenarioMode.MainExecutionContext;
            btModeAsyncPar.Click += (o, e) => this.SelectedMode = RunExistingScenarioMode.Asynchronously;
            btModeSync.Click += (o, e) => this.SelectedMode = RunExistingScenarioMode.Synchronously;

            btApply.Click += (o, e) => SelectionChanged?.Invoke(this);
        }

        public void Initialize(Type valueType = null, ActionInstanceSide side = ActionInstanceSide.Both, string selectedScenarioId = "", RunExistingScenarioMode runMode = RunExistingScenarioMode.Synchronously)
        {
            SelectedMode = runMode;            
            var scenarios = _repository.Scenarios.Where(x => valueType == null || x.ValueType.GetType().Equals(valueType));
            this.SelectedScenario = _repository.Scenarios.FirstOrDefault(x => x.Id.Equals(selectedScenarioId));
            if (side == ActionInstanceSide.OnlyRight)
                scenarios = scenarios.Where(x => x.ValueType is ButtonValueType == false);
            else if (side == ActionInstanceSide.OnlyLeft)
                scenarios = scenarios.Where(x => !x.OnlyGetValue);
            foreach (var scenario in scenarios)
            {
                var itemView = new ItemView();
                itemView.Content = scenario.Name;
                if (scenario.Id.Equals(selectedScenarioId))
                    this.Loaded += (o, e) => itemView.Selected = true;
                itemView.Icon = Icons.Icon.ChevronRight;
                itemView.Tag = scenario;
                itemView.Margin = new Thickness(2);
                itemView.Click += (o, e) => 
                {
                    this.SelectedScenario = itemView.Tag as ScenarioBase;
                };
                this.itemsView.Children.Add(itemView);
            }

            if (this.itemsView.Children.Count > 0)
            {
                tbScensNotExist.Visibility = Visibility.Collapsed;
                this.MinHeight = 0;
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

        public Action<SelectScenarioAndRunModeView> SelectionChanged;

        public static void Show(Action<ScenarioBase, RunExistingScenarioMode> selectedCallback, Type valueType = null, ActionInstanceSide side = ActionInstanceSide.Both, string selectedScenarioId = "", RunExistingScenarioMode runMode = RunExistingScenarioMode.Synchronously)
        {
            var control = new SelectScenarioAndRunModeView();
            var dialogView = new DialogView(control);
            control.Initialize(valueType, side, selectedScenarioId, runMode);
            control.SelectionChanged += (ctrl) =>
            {
                selectedCallback?.Invoke(control.SelectedScenario, control.SelectedMode);
                dialogView.Close();
            };
            dialogView.Show();
        }
    }
}