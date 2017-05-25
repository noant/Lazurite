using Lazurite.MainDomain;
using LazuriteMobile.App.Switches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace LazuriteMobile.App
{
    public partial class SwitchesGrid : ContentView
    {
        public SwitchesGrid()
        {
            InitializeComponent();
        }

        public void Initialize(ScenarioInfo[] scenarios, UserVisualSettings[] visualSettings)
        {
            foreach (var scenario in scenarios)
            {
                var visualSetting = visualSettings.FirstOrDefault(x => x.ScenarioId.Equals(scenario.ScenarioId));
                if (visualSetting == null)
                    visualSetting = new UserVisualSettings() { ScenarioId = scenario.ScenarioId };
                var control = SwitchesCreator.CreateScenarioControl(scenario, visualSetting);
                grid.Children.Add(control);
            }
            Rearrange();
        }

        public void Rearrange()
        {
            var maxX = 3;
            var marginLeft = 2;
            var marginTop = 2;
            var elementSize = 110;
            var occupiedPoints = new List<Point>();
            foreach (View control in grid.Children)
            {
                var scenario = ((ScenarioModel)control.BindingContext).Scenario;
                var visualSettings = ((ScenarioModel)control.BindingContext).VisualSettings;
                var targetPoint = new Point(visualSettings.PositionX, visualSettings.PositionY);
                while (occupiedPoints.Any(x => x.Equals(targetPoint)))
                {
                    targetPoint.X++;
                    if (targetPoint.X.Equals(maxX) || (targetPoint.X.Equals(maxX - 1) && control is FloatView))
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

                control.VerticalOptions = new LayoutOptions(LayoutAlignment.Start, false);
                control.HorizontalOptions = new LayoutOptions(LayoutAlignment.Start, false);
                control.WidthRequest = control.HeightRequest = elementSize;
                if (control is FloatView)
                    control.WidthRequest = elementSize * 2 + marginLeft;
                control.Margin = new Thickness(marginLeft * (1 + targetPoint.X) + elementSize * targetPoint.X, marginTop * (1 + targetPoint.Y) + elementSize * targetPoint.Y, 0, 0);
            }
        }
    }
}
