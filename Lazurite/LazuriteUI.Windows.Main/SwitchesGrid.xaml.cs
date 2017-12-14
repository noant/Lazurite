using Lazurite.IOC;
using Lazurite.MainDomain;
using Lazurite.Visual;
using LazuriteUI.Icons;
using LazuriteUI.Windows.Main.Switches;
using System;
using System.ComponentModel;
using System.Linq;
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
    public partial class SwitchesGrid : UserControl, IInitializable
    {
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

        private UsersRepositoryBase _usersRepository = Singleton.Resolve<UsersRepositoryBase>();
        private VisualSettingsRepository _visualSettingsRepository = Singleton.Resolve<VisualSettingsRepository>();
        private ScenariosRepositoryBase _scenariosRepository = Singleton.Resolve<ScenariosRepositoryBase>();
        private UserControl _draggableCurrent;

        public SwitchesGrid()
        {
            InitializeComponent();
            this.MouseMove += SwitchesGrid_MouseMove;
            this.MouseLeftButtonUp += ElementMouseRelease;
            this.grid.Margin = new Thickness(0, 0, ElementMargin, ElementMargin);
            this.Loaded += (o, e) => SetEditMode(this.EditMode); //crutch
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
            if (this.EditMode && _draggableCurrent != null)
            {
                var margin = ElementMargin;
                var elementSize = ElementSize;
                var position = e.GetPosition(this.grid);
                var positionExt = new Point(position.X / (elementSize + margin), position.Y / (elementSize + margin));
                var model = ((ScenarioModel)_draggableCurrent.DataContext);
                if ((model.PositionX != (int)positionExt.X || model.PositionY != (int)positionExt.Y) && (int)positionExt.X < MaxX)
                {
                    Move(_draggableCurrent, new Point((int)positionExt.X >= 0 ? (int)positionExt.X : 0, (int)positionExt.Y >= 0 ? (int)positionExt.Y : 0));
                }
            }
        }

        private void SetEditMode(bool value)
        {
            this.grid.Children
                .Cast<FrameworkElement>()
                .Select(x => ((ScenarioModel)x.DataContext).EditMode = value)
                .ToArray();
        }

        public void Initialize()
        {
            var scenarios = _scenariosRepository.Scenarios;
            if (!IsConstructorMode)
                scenarios = scenarios.Where(x => x.CanExecute(_usersRepository.SystemUser, ScenarioStartupSource.SystemUI)).ToArray();
            Initialize(scenarios, _visualSettingsRepository.VisualSettings);
        }

        private void Initialize(ScenarioBase[] scenarios, UserVisualSettings[] visualSettings)
        {
            foreach (var scenario in scenarios)
            {
                var visualSetting = visualSettings.FirstOrDefault(x => x.ScenarioId.Equals(scenario.Id));
                if (visualSetting == null)
                    visualSetting = new UserVisualSettings() { ScenarioId = scenario.Id, UserId = _usersRepository.SystemUser.Id };
                var control = SwitchesCreator.CreateScenarioControl(scenario, visualSetting);
                control.MouseLeftButtonDown += ElementClick;
                control.MouseLeftButtonUp += ElementMouseRelease;
                grid.Children.Add(control);
            }
            Rearrange();
            SelectFirst();
        }

        public void Add(ScenarioBase scenario, UserVisualSettings visualSettings)
        {
            if (visualSettings == null)
                visualSettings = new UserVisualSettings() { ScenarioId = scenario.Id, UserId = _usersRepository.SystemUser.Id };
            var control = SwitchesCreator.CreateScenarioControl(scenario, visualSettings);
            ((ScenarioModel)control.DataContext).EditMode = this.EditMode;
            control.MouseLeftButtonDown += ElementClick;
            control.MouseLeftButtonUp += ElementMouseRelease;
            grid.Children.Add(control);
            Rearrange();
            Select(scenario);
        }

        public void Remove(ScenarioBase scenario)
        {
            var control = grid.Children.Cast<UserControl>()
                .FirstOrDefault(x => ((ScenarioModel)x.DataContext).Scenario.Id.Equals(scenario.Id));
            grid.Children.Remove(control);
            Rearrange();
            SelectFirst();
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

        public void RefreshItemFull(ScenarioBase scenario)
        {
            var control = grid.Children.Cast<UserControl>()
                .FirstOrDefault(x => ((ScenarioModel)x.DataContext).Scenario.Id.Equals(scenario.Id));
            if (control != null)
            {
                var oldModel = (ScenarioModel)control.DataContext;
                grid.Children.Remove(control);
                control = SwitchesCreator.CreateScenarioControl(scenario, oldModel.VisualSettings);
                var model = (ScenarioModel)control.DataContext;
                model.EditMode = this.EditMode;
                control.MouseLeftButtonDown += ElementClick;
                control.MouseLeftButtonUp += ElementMouseRelease;
                grid.Children.Add(control);
                Rearrange();
                if (this.SelectedModel?.Scenario.Id == model.Scenario.Id)
                    SelectInternal(model);
            }
        }

        public void Select(ScenarioBase scenario)
        {
            var @switch = this.grid.Children.Cast<UserControl>().FirstOrDefault(control =>
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
            var firstSwitch = this.grid.Children.Cast<UserControl>().FirstOrDefault(control =>
            {
                var model = ((ScenarioModel)control.DataContext);
                return model.PositionX.Equals(0) && model.PositionY.Equals(0);
            });
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

        private void ElementMouseRelease(object sender, MouseButtonEventArgs e)
        {
            _draggableCurrent = null;
        }

        private void ElementClick(object sender, MouseEventArgs e)
        {
            if (this.EditMode)
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
                var maxX = MaxX;
                var margin = ElementMargin;
                var elementSize = ElementSize;
                foreach (UserControl control in grid.Children.Cast<UserControl>())
                {
                    var scenario = ((ScenarioModel)control.DataContext).Scenario;
                    var model = ((ScenarioModel)control.DataContext);
                    control.VerticalAlignment = VerticalAlignment.Top;
                    control.HorizontalAlignment = HorizontalAlignment.Left;
                    control.Width = control.Height = elementSize;
                }

                //optimize
                var controls = grid.Children.Cast<UserControl>().ToArray();
                var controlsModels = controls.Select(x => ((ScenarioModel)x.DataContext)).ToArray();
                
                var curX = 0;
                var curY = 0;
                foreach (var visualSetting in controlsModels
                    .OrderBy(x => x.Scenario.Id)
                    .OrderBy(x => x.ScenarioName)
                    .OrderBy(x => x.PositionX)
                    .OrderBy(x => x.PositionY))
                {
                    visualSetting.PositionX = curX;
                    visualSetting.PositionY = curY;
                    curX++;
                    if (curX == maxX)
                    {
                        curX = 0;
                        curY++;
                    }
                }

                //move
                foreach (var control in grid.Children.Cast<UserControl>())
                {
                    var model = ((ScenarioModel)control.DataContext);
                    control.Margin = new Thickness(
                        margin * (1 + model.PositionX) + elementSize * model.PositionX, 
                        margin * (1 + model.PositionY) + elementSize * model.PositionY, 0, 0);
                }
                ScenariosEmptyModeOff();
            }
            else
            {
                ScenariosEmptyModeOn();
            }
        }

        public void Move(UserControl control, Point position)
        {
            var model = ((ScenarioModel)control.DataContext);
            var controlAtPoint = this.grid.Children.Cast<UserControl>().FirstOrDefault(x =>
                ((ScenarioModel)x.DataContext).PositionX == position.X &&
                ((ScenarioModel)x.DataContext).PositionY == position.Y);
            if (controlAtPoint != null)
            {
                ((ScenarioModel)controlAtPoint.DataContext).PositionX = model.PositionX;
                ((ScenarioModel)controlAtPoint.DataContext).PositionY = model.PositionY;
            }

            model.PositionX = (int)position.X;
            model.PositionY = (int)position.Y;

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

        public void CancelDragging()
        {
            _draggableCurrent = null;
        }

        public event Action<ScenarioModel> SelectedModelChanged;
        public event Action<ScenarioModel, ScenarioChangingEventArgs> SelectedModelChanging;
    }

    public class ScenarioChangingEventArgs
    {
        public Action Apply { get; set; }
    }
}