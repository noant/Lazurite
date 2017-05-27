using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.IOC;
using Lazurite.MainDomain;
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

namespace LazuriteUI.Windows.Main
{
    /// <summary>
    /// Логика взаимодействия для SwitchesGrid.xaml
    /// </summary>
    public partial class SwitchesGrid : UserControl
    {
        public static DependencyProperty EditModeProperty;

        static SwitchesGrid()
        {
            EditModeProperty = DependencyProperty.Register(nameof(EditMode), typeof(bool), typeof(SwitchesGrid), new FrameworkPropertyMetadata() {
                DefaultValue = false,
                PropertyChangedCallback = (o,e) => {
                    ((SwitchesGrid)o).grid.Children
                        .Cast<FrameworkElement>()
                        .Select(x => ((ScenarioModel)x.DataContext).EditMode = (bool)e.NewValue)
                        .ToArray();
                }
            });
        }

        private UsersRepositoryBase _usersRepository;

        public SwitchesGrid()
        {
            InitializeComponent();
            this.MouseMove += SwitchesGrid_MouseMove;
            this.MouseLeftButtonUp += ElementMouseRelease;
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

        private void SwitchesGrid_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.EditMode && _draggableCurrent != null)
            {
                var marginLeft = 2;
                var marginTop = 2;
                var elementSize = 110;
                var position = e.GetPosition(this.grid);
                var positionExt = new Point(position.X / (elementSize + marginLeft), position.Y / (elementSize + marginTop));
                var visualSettings = ((ScenarioModel)_draggableCurrent.DataContext).VisualSettings;
                if (visualSettings.PositionX != (int)positionExt.X || visualSettings.PositionY != (int)positionExt.Y)
                {
                    Move(_draggableCurrent, new Point((int)positionExt.X >= 0 ? (int)positionExt.X : 0, (int)positionExt.Y >= 0 ? (int)positionExt.Y : 0));
                }
            }
        }

        public void Initialize(ScenarioBase[] scenarios, UserVisualSettings[] visualSettings)
        {
            _usersRepository = Singleton.Resolve<UsersRepositoryBase>();
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
        }

        private void ElementMouseRelease(object sender, MouseButtonEventArgs e)
        {
            _draggableCurrent = null;
        }

        private void ElementClick(object sender, MouseEventArgs e)
        {
            if (this.EditMode)
            {
                var control = sender as UserControl;
                var model = ((ScenarioModel)control.DataContext);
                model.Checked = true;
                foreach (UserControl userControl in grid.Children)
                {
                    if (userControl != control)
                        ((ScenarioModel)userControl.DataContext).Checked = false;
                }
                _draggableCurrent = control;                
            }
        }
        
        private UserControl _draggableCurrent;
        
        public void Rearrange()
        {
            var maxX = 4;
            var marginLeft = 2;
            var marginTop = 2;
            var elementSize = 110;
            var occupiedPoints = new List<Point>();
            foreach (UserControl control in grid.Children.Cast<UserControl>())
            {
                var scenario = ((ScenarioModel)control.DataContext).Scenario;
                var visualSettings = ((ScenarioModel)control.DataContext).VisualSettings;
                var targetPoint = new Point(visualSettings.PositionX, visualSettings.PositionY);
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
                visualSettings.PositionX = (int)targetPoint.X;
                visualSettings.PositionY = (int)targetPoint.Y;

                occupiedPoints.Add(targetPoint);

                control.VerticalAlignment = VerticalAlignment.Top;
                control.HorizontalAlignment = HorizontalAlignment.Left;
                control.Width = control.Height = elementSize;
            }
            //optimize
            var controls = grid.Children.Cast<UserControl>().ToArray();
            var controlsVisualSettings = controls.Select(x => ((ScenarioModel)x.DataContext).VisualSettings).ToArray();
            foreach (var visualSetting in controlsVisualSettings.OrderBy(x=>x.PositionY).OrderBy(x=>x.PositionX))
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
                        x = maxX;
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
                var targetPoint = new Point(model.VisualSettings.PositionX, model.VisualSettings.PositionY);
                control.Margin = new Thickness(marginLeft * (1 + targetPoint.X) + elementSize * targetPoint.X, marginTop * (1 + targetPoint.Y) + elementSize * targetPoint.Y, 0, 0);
            }
        }

        private bool IsPointOccupied(UserControl[] controls, int x, int y)
        {
            if (x < 0 || y < 0)
                return true;
            return controls.Any(control =>
            {
                var settings = ((ScenarioModel)control.DataContext).VisualSettings;
                return settings.PositionX.Equals(x) && settings.PositionY.Equals(y);
            });
        }

        public void Move(UserControl control, Point position)
        {
            var visualSettings = ((ScenarioModel)control.DataContext).VisualSettings;
            var controlAtPoint = this.grid.Children.Cast<UserControl>().FirstOrDefault(x =>
                ((ScenarioModel)x.DataContext).VisualSettings.PositionX == position.X &&
                ((ScenarioModel)x.DataContext).VisualSettings.PositionY == position.Y);
            if (controlAtPoint != null)
            {
                ((ScenarioModel)controlAtPoint.DataContext).VisualSettings.PositionX = visualSettings.PositionX;
                ((ScenarioModel)controlAtPoint.DataContext).VisualSettings.PositionY = visualSettings.PositionY;
            }

            visualSettings.PositionX = (int)position.X;
            visualSettings.PositionY = (int)position.Y;

            Rearrange();
        }
    }
}