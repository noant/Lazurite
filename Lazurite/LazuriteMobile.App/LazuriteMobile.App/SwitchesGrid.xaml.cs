using Lazurite.MainDomain;
using LazuriteMobile.App.Switches;
using Plugin.DeviceInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace LazuriteMobile.App
{
    public partial class SwitchesGrid : Grid
    {
        private static object Locker = new object();
        private static readonly int MaxX = 3;
        private static readonly int ElementSize = 111;
        private static readonly int ElementMargin = 6;
        
        public SwitchesGrid()
        {
            InitializeComponent();
            ScenariosEmptyModeOff();
            this.grid.Margin = new Thickness(0, 0, ElementMargin, 40);
        }

        private View CreateControl(ScenarioInfo scenarioInfo)
        {
            return SwitchesCreator.CreateScenarioControl(scenarioInfo);
        }

        public void Initialize(ScenarioInfo[] scenarios)
        {
            this.grid.Children.Clear();
            foreach (var scenario in scenarios)
                grid.Children.Add(CreateControl(scenario));
            Rearrange();
        }
        
        public void Refresh(ScenarioInfo[] scenarios)
        {
            lock (Locker)
            {
                var modelsViews = grid.Children.ToDictionary(x => (SwitchScenarioModel)x.BindingContext).ToList();
                var models = modelsViews.Select(x => x.Key).ToArray();
                //add new scenarios and refresh existing
                foreach (var scenario in scenarios)
                {
                    var scenarioModel = models.FirstOrDefault(x => x.Scenario.ScenarioId.Equals(scenario.ScenarioId));
                    if (scenarioModel != null)
                    {
                        scenarioModel.RefreshWith(scenario);
                    }
                    else
                    {
                        var control = CreateControl(scenario);
                        this.grid.Children.Add(control);
                    }
                }

                //remove not existing scenarios
                foreach (var modelView in modelsViews)
                {
                    if (!scenarios.Any(x => x.ScenarioId.Equals(modelView.Key.Scenario.ScenarioId)))
                        grid.Children.Remove(modelView.Value);
                }

                Rearrange();
            }
        }

        public void RefreshLE(ScenarioInfo[] scenarios)
        {
            lock (Locker)
            {
                var modelsViews = grid.Children.ToDictionary(x => (SwitchScenarioModel)x.BindingContext).ToList();
                var models = modelsViews.Select(x => x.Key).ToArray();
                //add new scenarios and refresh existing
                foreach (var scenario in scenarios)
                {
                    var scenarioModel = models.FirstOrDefault(x => x.Scenario.ScenarioId.Equals(scenario.ScenarioId));
                    if (scenarioModel != null)
                    {
                        scenarioModel.RefreshWith(scenario);
                    }
                    else
                    {
                        var control = CreateControl(scenario);
                        this.grid.Children.Add(control);
                    }
                }

                Rearrange();
            }
        }

        public void Rearrange()
        {
            if (grid.Children.Any())
            {
                var maxX = MaxX;
                var margin = ElementMargin;
                var elementSize = ElementSize;
                var occupiedPoints = new List<Point>();
                foreach (View control in grid.Children)
                {
                    var scenario = ((SwitchScenarioModel)control.BindingContext).Scenario;
                    var visualSettings = ((SwitchScenarioModel)control.BindingContext).VisualSettings;
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

                    control.VerticalOptions = new LayoutOptions(LayoutAlignment.Start, false);
                    control.HorizontalOptions = new LayoutOptions(LayoutAlignment.Start, false);
                    control.WidthRequest = control.HeightRequest = elementSize;
                }

                //optimize
                var controls = grid.Children.ToArray();
                var controlsVisualSettings = controls.Select(x => ((SwitchScenarioModel)x.BindingContext).VisualSettings).ToArray();

                var curX = 0;
                var curY = 0;
                foreach (var visualSetting in controlsVisualSettings.OrderBy(x => x.PositionX).OrderBy(x => x.PositionY))
                {
                    visualSetting.PositionX = curX;
                    visualSetting.PositionY = curY;
                    curX++;
                    if (curX == 3)
                    {
                        curX = 0;
                        curY++;
                    }
                }

                //move
                foreach (var control in controls)
                {
                    var model = ((SwitchScenarioModel)control.BindingContext);
                    var targetPoint = new Point(model.VisualSettings.PositionX, model.VisualSettings.PositionY);
                    control.Margin = new Thickness(margin * (1 + targetPoint.X) + elementSize * targetPoint.X, margin * (1 + targetPoint.Y) + elementSize * targetPoint.Y, 0, 0);
                }

                ScenariosEmptyModeOff();
            }
            else
            {
                ScenariosEmptyModeOn();
            }
        }

        private bool IsPointOccupied(View[] controls, int x, int y)
        {
            if (x < 0 || y < 0 || x >= MaxX-1)
                return true;
            return controls.Any(control =>
            {
                var settings = ((SwitchScenarioModel)control.BindingContext).VisualSettings;
                return settings.PositionX.Equals(x) && settings.PositionY.Equals(y);
            });
        }
        
        public void ScenariosEmptyModeOn()
        {
            lblEmpty.IsVisible = true;
        }

        public void ScenariosEmptyModeOff()
        {
            lblEmpty.IsVisible = false;
        }
    }
}