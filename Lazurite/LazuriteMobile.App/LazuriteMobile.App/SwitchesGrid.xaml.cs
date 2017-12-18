using Lazurite.MainDomain;
using LazuriteMobile.App.Switches;
using System.Linq;

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
                this.BatchBegin();
                
                var modelsViews = grid.Children.ToDictionary(x => (SwitchScenarioModel)x.BindingContext);
                var models = modelsViews.Select(x => x.Key).ToArray();
                //add new scenarios and refresh existing
                foreach (var scenario in scenarios)
                {
                    var scenarioModel = models.FirstOrDefault(x => x.Scenario.ScenarioId.Equals(scenario.ScenarioId));
                    if (scenarioModel != null && 
                        scenarioModel.Scenario.ValueType.Equals(scenario.ValueType))
                    {
                        var control = modelsViews[scenarioModel];
                        this.grid.Children.Remove(control);
                        control = CreateControl(scenario);
                        this.grid.Children.Add(control);
                    }
                    else if (scenarioModel != null)
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

                this.BatchCommit();
            }
        }

        public void RefreshLE(ScenarioInfo[] scenarios)
        {
            lock (Locker)
            {
                this.BatchBegin();

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

                this.BatchCommit();
            }
        }
        
        public void Rearrange()
        {
            if (grid.Children.Any())
            {
                var maxX = MaxX;
                var margin = ElementMargin;
                var elementSize = ElementSize;
                foreach (View control in grid.Children)
                {
                    var scenario = ((SwitchScenarioModel)control.BindingContext).Scenario;
                    var visualSettings = ((SwitchScenarioModel)control.BindingContext).VisualSettings;
                    control.VerticalOptions = new LayoutOptions(LayoutAlignment.Start, false);
                    control.HorizontalOptions = new LayoutOptions(LayoutAlignment.Start, false);
                    control.WidthRequest = control.HeightRequest = elementSize;
                }

                //optimize
                var controls = grid.Children.ToArray();
                var controlsModels = controls.Select(x => ((SwitchScenarioModel)x.BindingContext)).ToArray();

                var curX = 0;
                var curY = 0;
                foreach (var visualSetting in controlsModels
                    .OrderBy(x => x.VisualSettings.ScenarioId)
                    .OrderBy(x => x.ScenarioName)
                    .OrderBy(x => x.VisualSettings.PositionX)
                    .OrderBy(x => x.VisualSettings.PositionY)
                    .Select(x=>x.VisualSettings))
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
                foreach (var control in controls)
                {
                    var model = ((SwitchScenarioModel)control.BindingContext);                    
                    control.Margin = new Thickness(
                        margin * (1 + model.VisualSettings.PositionX) + elementSize * model.VisualSettings.PositionX, 
                        margin * (1 + model.VisualSettings.PositionY) + elementSize * model.VisualSettings.PositionY, 0, 0);
                }

                ScenariosEmptyModeOff();
            }
            else
            {
                ScenariosEmptyModeOn();
            }
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