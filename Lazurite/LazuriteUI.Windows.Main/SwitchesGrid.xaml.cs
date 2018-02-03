using Lazurite.IOC;
using Lazurite.MainDomain;
using Lazurite.Visual;
using LazuriteUI.Icons;
using LazuriteUI.Windows.Main.Switches;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LazuriteUI.Windows.Main
{
    /// <summary>
    /// Логика взаимодействия для SwitchesGrid.xaml
    /// </summary>
    [DisplayName("Переключатели сценариев")]
    [LazuriteIcon(Icon.CursorHand)]
    public partial class SwitchesGrid : UserControl, IInitializable, IDisposable
    {
        private static readonly int UpdateUIInterval_MS = GlobalSettings.Get(30000);
        private static readonly ISystemUtils SystemUtils = Singleton.Resolve<ISystemUtils>();

        private static readonly UsersRepositoryBase UsersRepository = Singleton.Resolve<UsersRepositoryBase>();
        private static readonly ScenariosRepositoryBase ScenariosRepository = Singleton.Resolve<ScenariosRepositoryBase>();
        private static readonly VisualSettingsRepository VisualSettingsRepository = Singleton.Resolve<VisualSettingsRepository>();

        public static DependencyProperty EditModeProperty;
        public static DependencyProperty EditModeButtonVisibleProperty;
        public static DependencyProperty IsConstructorModeProperty;

        private static readonly int MaxX = 3;
        private static readonly int ElementSize = 111;
        private static readonly int ElementMargin = 6;

        static SwitchesGrid()
        {
            EditModeProperty = DependencyProperty.Register(nameof(EditMode), typeof(bool), typeof(SwitchesGrid), new FrameworkPropertyMetadata() {
                DefaultValue = false,
                PropertyChangedCallback = (o, e) => {
                    ((SwitchesGrid)o).SetEditMode((bool)e.NewValue);
                }
            });

            EditModeButtonVisibleProperty = DependencyProperty.Register(nameof(EditModeButtonVisible), typeof(bool), typeof(SwitchesGrid), new FrameworkPropertyMetadata(true));
            IsConstructorModeProperty = DependencyProperty.Register(nameof(IsConstructorMode), typeof(bool), typeof(SwitchesGrid), new FrameworkPropertyMetadata(false));
        }

        private UserControl _draggableCurrent;
        private CancellationTokenSource _updateUICancellationToken = new CancellationTokenSource();

        public SwitchesGrid()
        {
            InitializeComponent();
            MouseMove += SwitchesGrid_MouseMove;
            MouseLeftButtonUp += ElementMouseRelease;
            grid.Margin = new Thickness(0, 0, ElementMargin, ElementMargin);
            Loaded += (o, e) => SetEditMode(EditMode); //crutch
        }
        
        public bool IsConstructorMode
        {
            get
            {
                return (bool)GetValue(IsConstructorModeProperty);
            }
            set
            {
                SetValue(IsConstructorModeProperty, value);
            }
        }

        public bool EditMode
        {
            get
            {
                return (bool)GetValue(EditModeProperty);
            }
            set
            {
                SetValue(EditModeProperty, value);
            }
        }

        public bool EditModeButtonVisible
        {
            get
            {
                return (bool)GetValue(EditModeButtonVisibleProperty);
            }
            set
            {
                SetValue(EditModeButtonVisibleProperty, value);
            }
        }

        public ScenarioModel SelectedModel
        {
            get; private set;
        }

        private void SwitchesGrid_MouseMove(object sender, MouseEventArgs e)
        {
            if (EditMode && _draggableCurrent != null)
            {
                var margin = ElementMargin;
                var elementSize = ElementSize;
                var position = e.GetPosition(grid);
                var potentialPosition = new Point(position.X / (elementSize + margin), position.Y / (elementSize + margin));
                var model = ((ScenarioModel)_draggableCurrent.DataContext);
                var currentPosition = CreatePositionByIndex(CreateRealVisualIndex(model.VisualSettings));
                if ((currentPosition.Item1 != (int)potentialPosition.X || currentPosition.Item2 != (int)potentialPosition.Y) && (int)potentialPosition.X < MaxX)
                {
                    Move(_draggableCurrent, new Point((int)potentialPosition.X >= 0 ? (int)potentialPosition.X : 0, (int)potentialPosition.Y >= 0 ? (int)potentialPosition.Y : 0));
                }
            }
        }

        private void SetEditMode(bool value)
        {
            grid.Children
                .Cast<FrameworkElement>()
                .Select(x => ((ScenarioModel)x.DataContext).EditMode = value)
                .ToArray();
        }

        public void Initialize()
        {            
            Initialize(GetScenarios());

            _updateUICancellationToken = SystemUtils.StartTimer(
                (token) => {
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        foreach (var scenario in GetScenarios())
                            RefreshAndReCalculateItem(scenario);
                    }));
                },
                () => UpdateUIInterval_MS);
        }

        private ScenarioBase[] GetScenarios()
        {
            var scenarios = ScenariosRepository.Scenarios;
            if (!IsConstructorMode)
                scenarios = scenarios.Where(x => x.CanExecute(UsersRepository.SystemUser, ScenarioStartupSource.SystemUI)).ToArray();
            return scenarios;
        }

        private void Initialize(ScenarioBase[] scenarios)
        {
            if (scenarios.Any())
            {
                foreach (var scenario in scenarios)
                {
                    var control = CreateControl(scenario);
                    grid.Children.Add(control);
                }
                ScenariosEmptyModeOff();
            }
            else
                ScenariosEmptyModeOn();
            SelectFirst();
        }
        
        private UserControl CreateControl(ScenarioBase scenario)
        {
            var control = SwitchesCreator.CreateScenarioControl(scenario);
            control.MouseLeftButtonDown += ElementClick;
            control.MouseLeftButtonUp += ElementMouseRelease;
            control.VerticalAlignment = VerticalAlignment.Top;
            control.HorizontalAlignment = HorizontalAlignment.Left;
            control.Width = control.Height = ElementSize;
            control.Margin = CreateControlMargin(GetVisualSettings(control));
            return control;
        }
        
        private UserVisualSettings GetVisualSettings(UserControl control)
        {
            var model = control.DataContext as ScenarioModel;
            return model?.VisualSettings;
        }

        public void Add(ScenarioBase scenario)
        {
            var control = CreateControl(scenario);
            ((ScenarioModel)control.DataContext).EditMode = EditMode;
            grid.Children.Add(control);
            Rearrange();
            Select(scenario);

            if (grid.Children.Count > 0)
                ScenariosEmptyModeOff();
        }

        public void Remove(ScenarioBase scenario)
        {
            var control = grid.Children.Cast<UserControl>()
                .FirstOrDefault(x => ((ScenarioModel)x.DataContext).Scenario.Id.Equals(scenario.Id));
            grid.Children.Remove(control);
            Rearrange();
            SelectFirst();

            if (grid.Children.Count == 0)
                ScenariosEmptyModeOn();
        }

        public void RefreshItem(ScenarioBase scenario)
        {
            var control = grid.Children.Cast<UserControl>()
                .FirstOrDefault(x => ((ScenarioModel)x.DataContext).Scenario.Id.Equals(scenario.Id));
            if (control != null)
            {
                var model = (ScenarioModel)control.DataContext;
                model.Refresh();
            }
        }

        public void RefreshAndReCalculateItem(ScenarioBase scenario)
        {
            var control = grid.Children.Cast<UserControl>()
                .FirstOrDefault(x => ((ScenarioModel)x.DataContext).Scenario.Id.Equals(scenario.Id));
            if (control != null)
            {
                var model = (ScenarioModel)control.DataContext;
                model.RefreshAndReCalculate();
            }
        }

        public void RefreshItemFull(ScenarioBase scenario)
        {
            var control = grid.Children.Cast<UserControl>()
                .FirstOrDefault(x => ((ScenarioModel)x.DataContext).Scenario.Id.Equals(scenario.Id));
            if (control != null)
            {
                grid.Children.Remove(control);
                control = CreateControl(scenario);
                var model = (ScenarioModel)control.DataContext;
                model.EditMode = EditMode;
                grid.Children.Add(control);
                Rearrange();
                if (SelectedModel?.Scenario.Id == model.Scenario.Id)
                    SelectInternal(model);
            }
        }

        public void Select(ScenarioBase scenario)
        {
            var @switch = grid.Children.Cast<UserControl>().FirstOrDefault(control =>
            {
                var model = ((ScenarioModel)control.DataContext);
                return model.Scenario.Id.Equals(scenario.Id);
            });
            if (@switch != null)
            {
                var model = ((ScenarioModel)@switch.DataContext);
                if (model?.Scenario.Id != SelectedModel?.Scenario.Id || SelectedModel == null)
                {
                    SelectInternal(model);
                }
            }
        }

        private void SelectInternal(ScenarioModel model)
        {
            if (SelectedModelChanging != null)
            {
                SelectedModelChanging?.Invoke(
                    model,
                    new ScenarioChangingEventArgs()
                    {
                        Apply = () =>
                        {
                            if (model != null)
                                model.Checked = true;
                            BindSwitchSettings(model);
                            SelectedModel = model;
                            SelectedModelChanged?.Invoke(model);
                        }
                    });
            }
            else
            {
                if (model != null)
                    model.Checked = true;
                BindSwitchSettings(model);
                SelectedModel = model;
                SelectedModelChanged?.Invoke(model);
            }
        }

        private void SelectFirst()
        {
            if (grid.Children.Count == 0)
                SelectInternal(null);
            else
            {
                var minIndex = grid.Children.Cast<UserControl>().Min(x => ((ScenarioModel)x.DataContext).VisualIndex);
                var firstSwitch = grid.Children.Cast<UserControl>().FirstOrDefault(x => ((ScenarioModel)x.DataContext).VisualIndex.Equals(minIndex));
                if (firstSwitch != null)
                {
                    var model = ((ScenarioModel)firstSwitch.DataContext);
                    SelectInternal(model);
                }
                else
                {
                    SelectInternal(null);
                }
            }
        }

        private void ElementMouseRelease(object sender, MouseButtonEventArgs e)
        {
            _draggableCurrent = null;
        }

        private void ElementClick(object sender, MouseEventArgs e)
        {
            if (EditMode)
            {
                if (_draggableCurrent != sender)
                {
                    _draggableCurrent = (UserControl)sender;
                    var model = (ScenarioModel)_draggableCurrent.DataContext;
                    if (model?.Scenario.Id != SelectedModel?.Scenario.Id || SelectedModel == null)
                    {
                        SelectInternal(model);
                    }
                }
            }
        }

        private void BindSwitchSettings(ScenarioModel model)
        {
            foreach (UserControl userControl in grid.Children)
            {
                if (userControl.DataContext != model)
                    ((ScenarioModel)userControl.DataContext).Checked = false;
            }
            switchSetting.DataContext = model;
        }

        public void Rearrange()
        {
            if (grid.Children.Count > 0)
            {
                foreach (UserControl control in grid.Children.Cast<UserControl>())
                {
                    var model = ((ScenarioModel)control.DataContext);
                    control.Margin = CreateControlMargin(model.VisualSettings);
                }
            }
        }

        public void Move(UserControl control, Point position)
        {
            var model = ((ScenarioModel)control.DataContext);
            int index = -1;
            var controlAtPoint = grid.Children
                .Cast<UserControl>()
                .FirstOrDefault(x =>
                {
                    index = CreateRealVisualIndex(((ScenarioModel)x.DataContext).VisualSettings);
                    var virtualPosition = CreatePositionByIndex(index);
                    return virtualPosition.Item1.Equals((int)position.X) && virtualPosition.Item2.Equals((int)position.Y);
                });
            if (controlAtPoint != null)
            {
                var ordered = grid.Children
                    .Cast<UserControl>()
                    .Select(x => (ScenarioModel)x.DataContext)
                    .OrderBy(x => x.VisualSettings.VisualIndex)
                    .ToList();

                ordered.Remove(model);
                ordered.Insert(index, model);
                int newIndex = 0;
                foreach (var orderedModel in ordered)
                    orderedModel.VisualIndex = newIndex++;

                VisualSettingsRepository.Save();
            }

            Rearrange();
        }

        public void ScenariosEmptyModeOn()
        {
            tbEmpty.Visibility = Visibility.Visible;
            grid.Visibility = Visibility.Collapsed;
            switchSettingsHolder.Visibility = Visibility.Collapsed;
            EditModeButtonVisible = false;
        }

        public void ScenariosEmptyModeOff()
        {
            tbEmpty.Visibility = Visibility.Collapsed;
            grid.Visibility = Visibility.Visible;
            switchSettingsHolder.Visibility = Visibility.Visible;
        }

        private Tuple<int,int> CreatePositionByIndex(int visualIndex)
        {
            return new Tuple<int, int>(visualIndex % MaxX, visualIndex / MaxX);
        }

        private int CreateRealVisualIndex(UserVisualSettings visualSettings)
        {
            var scenarios = ScenariosRepository.Scenarios;
            if (!IsConstructorMode)
                scenarios = scenarios.Where(x => x.CanExecute(UsersRepository.SystemUser, ScenarioStartupSource.SystemUI)).ToArray();

            var allVisualSettings = 
                VisualSettingsRepository
                .VisualSettings
                .Where(x => x.UserId.Equals(UsersRepository.SystemUser.Id) && scenarios.Any(z=>z.Id.Equals(x.ScenarioId)))
                .OrderBy(z => z.VisualIndex)
                .ToList();

            var realVisualIndex = allVisualSettings.IndexOf(visualSettings);
            return realVisualIndex;
        }

        private Thickness CreateControlMargin(UserVisualSettings visualSettings)
        {            
            var position = CreatePositionByIndex(CreateRealVisualIndex(visualSettings));
            
            return new Thickness(
                ElementMargin * (1 + position.Item1) + ElementSize * position.Item1,
                ElementMargin * (1 + position.Item2) + ElementSize * position.Item2, 0, 0);
        }

        public void CancelDragging() => _draggableCurrent = null;

        public void Dispose() => _updateUICancellationToken.Cancel();

        public event Action<ScenarioModel> SelectedModelChanged;
        public event Action<ScenarioModel, ScenarioChangingEventArgs> SelectedModelChanging;
    }

    public class ScenarioChangingEventArgs
    {
        public Action Apply { get; set; }
    }
}