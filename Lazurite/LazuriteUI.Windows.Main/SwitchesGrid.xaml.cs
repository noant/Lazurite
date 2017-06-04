using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.IOC;
using Lazurite.MainDomain;
using Lazurite.Visual;
using LazuriteUI.Icons;
using LazuriteUI.Windows.Main.Switches;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace LazuriteUI.Windows.Main
{
    /// <summary>
    /// Логика взаимодействия для SwitchesGrid.xaml
    /// </summary>
    [DisplayName("Переключатели сценариев")]
    [LazuriteIcon(Icon.CursorHand)]
    public partial class SwitchesGrid : UserControl
    {
        public static DependencyProperty EditModeProperty;
        public static DependencyProperty EditModeButtonVisibleProperty;

        private static readonly int MaxX = 3;
        private static readonly int ElementSize = 111;
        private static readonly int ElementMargin = 6;

        static SwitchesGrid()
        {
            EditModeProperty = DependencyProperty.Register(nameof(EditMode), typeof(bool), typeof(SwitchesGrid), new FrameworkPropertyMetadata() {
                DefaultValue = false,
                PropertyChangedCallback = (o, e) => {
                    var swgrid = ((SwitchesGrid)o);
                    swgrid.grid.Children
                        .Cast<FrameworkElement>()
                        .Select(x => ((ScenarioModel)x.DataContext).EditMode = (bool)e.NewValue)
                        .ToArray();
                }
            });

            EditModeButtonVisibleProperty = DependencyProperty.Register(nameof(EditModeButtonVisible), typeof(bool), typeof(SwitchesGrid), new FrameworkPropertyMetadata(true));
        }

        private UsersRepositoryBase _usersRepository = Singleton.Resolve<UsersRepositoryBase>();
        private VisualSettingsRepository _visualSettingsRepository = Singleton.Resolve<VisualSettingsRepository>();
        private ScenariosRepositoryBase _scenariosRepository = Singleton.Resolve<ScenariosRepositoryBase>();

        public SwitchesGrid()
        {
            InitializeComponent();
            this.MouseMove += SwitchesGrid_MouseMove;
            this.MouseLeftButtonUp += ElementMouseRelease;
            this.grid.Margin = new Thickness(0, 0, ElementMargin, ElementMargin);
            Initialize(_scenariosRepository.Scenarios, _visualSettingsRepository.VisualSettings);
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

        public void Initialize(ScenarioBase[] scenarios, UserVisualSettings[] visualSettings)
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
                if (scenario.Equals(scenarios.First()))
                {
                    var model = control.DataContext as ScenarioModel;
                    model.Checked = true;
                    BindSwitchSettings(control);
                }
            }
            Rearrange();
        }

        private void ElementMouseRelease(object sender, MouseButtonEventArgs e)
        {
            _draggableCurrent = null;
        }

        private void ElementClick(object sender, MouseEventArgs e)
        {
            if (this.EditMode)
            {
                BindSwitchSettings((UserControl)sender);
                _draggableCurrent = (UserControl)sender;
            }
        }

        private void BindSwitchSettings(UserControl control)
        {
            var model = ((ScenarioModel)control.DataContext);
            model.Checked = true;
            foreach (UserControl userControl in grid.Children)
            {
                if (userControl != control)
                    ((ScenarioModel)userControl.DataContext).Checked = false;
            }
            switchSetting.DataContext = control.DataContext;
        }

        private UserControl _draggableCurrent;

        public void Rearrange()
        {
            var maxX = MaxX;
            var margin = ElementMargin;
            var elementSize = ElementSize;
            var occupiedPoints = new List<Point>();
            foreach (UserControl control in grid.Children.Cast<UserControl>())
            {
                var scenario = ((ScenarioModel)control.DataContext).Scenario;
                var model = ((ScenarioModel)control.DataContext);
                var targetPoint = new Point(model.PositionX, model.PositionY);
                while (occupiedPoints.Any(x => x.Equals(targetPoint)))
                {
                    targetPoint.X++;
                    if (targetPoint.X.Equals(maxX))
                    {
                        targetPoint.X = 0;
                        targetPoint.Y++;
                    }
                    else if (control is FloatView)
                        targetPoint.X++;
                }
                model.PositionX = (int)targetPoint.X;
                model.PositionY = (int)targetPoint.Y;

                occupiedPoints.Add(targetPoint);

                control.VerticalAlignment = VerticalAlignment.Top;
                control.HorizontalAlignment = HorizontalAlignment.Left;
                control.Width = control.Height = elementSize;
            }
            //optimize
            var controls = grid.Children.Cast<UserControl>().ToArray();
            var controlsModels = controls.Select(x => ((ScenarioModel)x.DataContext)).ToArray();
            foreach (var visualSetting in controlsModels.OrderBy(x => x.PositionY).OrderBy(x => x.PositionX))
            {
                var x = visualSetting.PositionX;
                var y = visualSetting.PositionY;

                var prevX = x;
                var prevY = y;

                do
                {
                    prevX = x;
                    prevY = y;
                    if (x == 0 && y != 0)
                    {
                        y--;
                        x = maxX - 1;
                    }
                    else if (x != 0)
                        x--;
                }
                while (!IsPointOccupied(controls, x, y) && !(prevX == 0 && prevY == 0));

                visualSetting.PositionX = prevX;
                visualSetting.PositionY = prevY;
            }

            //move
            foreach (var control in grid.Children.Cast<UserControl>())
            {
                var model = ((ScenarioModel)control.DataContext);
                var targetPoint = new Point(model.PositionX, model.PositionY);
                control.Margin = new Thickness(margin * (1 + targetPoint.X) + elementSize * targetPoint.X, margin * (1 + targetPoint.Y) + elementSize * targetPoint.Y, 0, 0);
            }
        }

        private bool IsPointOccupied(UserControl[] controls, int x, int y)
        {
            if (x < 0 || y < 0)
                return true;
            return controls.Any(control =>
            {
                var model = ((ScenarioModel)control.DataContext);
                return model.PositionX.Equals(x) && model.PositionY.Equals(y);
            });
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
    }
}