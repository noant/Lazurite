﻿using Lazurite.MainDomain;
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
        public static BindableProperty EditModeProperty;

        private static readonly int MaxX = 3;
        private static readonly int ElementSize = 111;
        private static readonly int ElementMargin = 6;

        static SwitchesGrid()
        {
            EditModeProperty = BindableProperty.Create(nameof(EditMode), typeof(bool), typeof(SwitchesGrid), false, BindingMode.Default, null,
                (o,oldVal,newVal) =>
                {
                    foreach (var view in ((SwitchesGrid)o).grid.Children)
                        ((ScenarioModel)view.BindingContext).EditMode = (bool)newVal;
                });
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

        public SwitchesGrid()
        {
            InitializeComponent();
            this.grid.Margin = new Thickness(0, 0, ElementMargin, 0);
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
            var maxX = MaxX;
            var margin = ElementMargin;
            var elementSize = ElementSize;
            var occupiedPoints = new List<Point>();
            foreach (View control in grid.Children)
            {
                var scenario = ((ScenarioModel)control.BindingContext).Scenario;
                var visualSettings = ((ScenarioModel)control.BindingContext).VisualSettings;
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
            var controlsVisualSettings = controls.Select(x => ((ScenarioModel)x.BindingContext).VisualSettings).ToArray();
            foreach (var visualSetting in controlsVisualSettings.OrderBy(x => x.PositionY).OrderBy(x => x.PositionX))
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
                        x = maxX-1;
                    }
                    else if (x != 0)
                        x--;
                }
                while (!IsPointOccupied(controls, x, y) && !(prevX == 0 && prevY == 0));

                visualSetting.PositionX = prevX;
                visualSetting.PositionY = prevY;
            }

            //move
            foreach (var control in controls)
            {
                var model = ((ScenarioModel)control.BindingContext);
                var targetPoint = new Point(model.VisualSettings.PositionX, model.VisualSettings.PositionY);
                control.Margin = new Thickness(margin * (1 + targetPoint.X) + elementSize * targetPoint.X, margin * (1 + targetPoint.Y) + elementSize * targetPoint.Y, 0, 0);
            }
        }

        private bool IsPointOccupied(View[] controls, int x, int y)
        {
            if (x < 0 || y < 0 || x >= MaxX-1)
                return true;
            return controls.Any(control =>
            {
                var settings = ((ScenarioModel)control.BindingContext).VisualSettings;
                return settings.PositionX.Equals(x) && settings.PositionY.Equals(y);
            });
        }

        public void Move(View control, Point position)
        {
            var visualSettings = ((ScenarioModel)control.BindingContext).VisualSettings;
            var controlAtPoint = this.grid.Children.FirstOrDefault(x =>
                ((ScenarioModel)x.BindingContext).VisualSettings.PositionX == position.X &&
                ((ScenarioModel)x.BindingContext).VisualSettings.PositionY == position.Y);

            if (controlAtPoint != null)
            {
                ((ScenarioModel)controlAtPoint.BindingContext).VisualSettings.PositionX = visualSettings.PositionX;
                ((ScenarioModel)controlAtPoint.BindingContext).VisualSettings.PositionY = visualSettings.PositionY;
            }

            visualSettings.PositionX = (int)position.X;
            visualSettings.PositionY = (int)position.Y;

            Rearrange();
        }
    }
}
