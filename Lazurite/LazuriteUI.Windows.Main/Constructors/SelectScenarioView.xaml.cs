using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.CoreActions.CoreActions;
using Lazurite.IOC;
using Lazurite.MainDomain;
using Lazurite.Windows.Modules;
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
    /// Логика взаимодействия для SelectScenarioView.xaml
    /// </summary>
    public partial class SelectScenarioView : UserControl
    {
        private ScenariosRepositoryBase _repository = Singleton.Resolve<ScenariosRepositoryBase>();
        
        public ScenarioBase SelectedScenario { get; private set; }

        public SelectScenarioView()
        {
            InitializeComponent();
            Loaded += (sender, e) => MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
        }

        public void Initialize(Type valueType = null, ActionInstanceSide side = ActionInstanceSide.Both, string selectedScenarioId = "")
        {
            var scenarios = _repository.Scenarios.Where(x => valueType == null || x.ValueType.GetType().Equals(valueType));
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
                    SelectionChanged?.Invoke(this);
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