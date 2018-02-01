using Lazurite.MainDomain;
using LazuriteMobile.App.Switches;
using System;
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
         
        private ScenarioInfo[] GetCurrentScenarios()
        {
            return grid.Children.Select(x => ((SwitchScenarioModel)x.BindingContext).Scenario).ToArray();
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

        public void Rearrange()
        {
            if (grid.Children.Count > 0)
            {
                foreach (View control in grid.Children)
                {
                    var model = ((SwitchScenarioModel)control.BindingContext);
                    control.Margin = CreateControlMargin(model.VisualSettings);
                }
                ScenariosEmptyModeOff();
            }
            else
            {
                ScenariosEmptyModeOn();
            }
        }

        private View CreateControl(ScenarioInfo scenario)
        {
            var control = SwitchesCreator.CreateScenarioControl(scenario);
            control.VerticalOptions = LayoutOptions.Start;
            control.HorizontalOptions = LayoutOptions.Start;
            control.WidthRequest = control.HeightRequest = ElementSize;
            control.Margin = CreateControlMargin(scenario.VisualSettings);
            return control;
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

        private Tuple<int, int> CreatePositionByIndex(int visualIndex)
        {
            return new Tuple<int, int>(visualIndex % MaxX, visualIndex / MaxX);
        }

        private Thickness CreateControlMargin(UserVisualSettings visualSettings)
        {
            var allVisualSettings =
                GetCurrentScenarios()
                .Select(x => x.VisualSettings)
                .OrderBy(z => z.VisualIndex)
                .ToList();

            var realVisualIndex = allVisualSettings.IndexOf(visualSettings);
            var position = CreatePositionByIndex(realVisualIndex);

            return new Thickness(
                ElementMargin * (1 + position.Item1) + ElementSize * position.Item1,
                ElementMargin * (1 + position.Item2) + ElementSize * position.Item2, 0, 0);
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