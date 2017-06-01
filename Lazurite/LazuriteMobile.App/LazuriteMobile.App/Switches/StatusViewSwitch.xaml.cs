﻿using LazuriteMobile.App.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace LazuriteMobile.App.Switches
{
    public partial class StatusViewSwitch : Grid
    {
        public StatusViewSwitch()
        {
            InitializeComponent();
        }

        public StatusViewSwitch(ScenarioModel scenarioModel) : this()
        {
            this.tbScenarioName.Text = scenarioModel.ScenarioName;
            foreach (var state in scenarioModel.AcceptedValues)
            {
                var itemView = new ItemView();
                itemView.Icon = LazuriteUI.Icons.Icon.NavigateNext;
                itemView.Text = state;
                if (scenarioModel.ScenarioValue.Equals(state))
                    itemView.Selected = true;
                listItemsStates.Children.Add(itemView);
            }

            listItemsStates.SelectionChanged += (o, e) =>
            {
                var selectedItem = listItemsStates.GetSelectedItems().FirstOrDefault() as ItemView;
                if (selectedItem != null && selectedItem.Text != scenarioModel.ScenarioValue)
                {
                    scenarioModel.ScenarioValue = selectedItem.Text;
                    StateChanged?.Invoke(this, new EventArgs());
                }
            };
        }

        public event Action<object, EventArgs> StateChanged;
    }
}
