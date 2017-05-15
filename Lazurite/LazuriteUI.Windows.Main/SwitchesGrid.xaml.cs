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
        private UsersRepositoryBase _usersRepository;

        public SwitchesGrid()
        {
            InitializeComponent();
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
                grid.Children.Add(control);
            }
            Rearrange();
        }

        public void Rearrange()
        {
            var maxX = 4;
            var marginLeft = 2;
            var marginTop = 2;
            var elementSize = 80;
            var occupiedPoints = new List<Point>();
            foreach (UserControl control in grid.Children)
            {
                var scenario = ((ScenarioModel)control.DataContext).Scenario;
                var visualSettings = ((ScenarioModel)control.DataContext).VisualSettings;
                var targetPoint = new Point(visualSettings.PositionX, visualSettings.PositionY);
                while (occupiedPoints.Any(x => x.Equals(targetPoint)))
                {
                    targetPoint.X++;
                    if (targetPoint.X.Equals(maxX) || (targetPoint.X.Equals(maxX-1) && control is FloatView))
                    {
                        targetPoint.X = 0;
                        targetPoint.Y++;
                    }
                }
                visualSettings.PositionX = (int)targetPoint.X;
                visualSettings.PositionY = (int)targetPoint.Y;

                occupiedPoints.Add(targetPoint);
                if (control is FloatView)
                    occupiedPoints.Add(new Point(targetPoint.X + 1, targetPoint.Y));

                control.VerticalAlignment = VerticalAlignment.Top;
                control.HorizontalAlignment = HorizontalAlignment.Left;
                control.Width = control.Height = elementSize;
                if (control is FloatView)
                    control.Width = elementSize * 2 + marginLeft;
                control.Margin = new Thickness(marginLeft * (1 + targetPoint.X) + elementSize * targetPoint.X, marginTop * (1 + targetPoint.Y) + elementSize * targetPoint.Y, 0, 0); 
            }
        }
    }
}