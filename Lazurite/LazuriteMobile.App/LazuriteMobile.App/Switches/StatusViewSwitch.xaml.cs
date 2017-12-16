﻿using Lazurite.IOC;
using Lazurite.MainDomain;
using Lazurite.Shared;
using Lazurite.Utils;
using LazuriteMobile.App.Controls;
using System;
using System.Linq;
using System.Threading;
using Xamarin.Forms;

namespace LazuriteMobile.App.Switches
{
    public partial class StatusViewSwitch : Grid
    {
        private static readonly int FloatView_ValueUpdateInterval = GlobalSettings.Get(300);
        private static ISystemUtils SystemUtils = Singleton.Resolve<ISystemUtils>();

        private CancellationTokenSource _tokenSource = new CancellationTokenSource();
        private IHardwareVolumeChanger _changer;

        public StatusViewSwitch()
        {
            InitializeComponent();
            if (Singleton.Any<IHardwareVolumeChanger>())
            {
                _changer = Singleton.Resolve<IHardwareVolumeChanger>();
                _changer.VolumeDown += _changer_VolumeChanged;
                _changer.VolumeUp += _changer_VolumeChanged;
            }
        }

        private void _changer_VolumeChanged(object sender, EventsArgs<int> args)
        {
            //megaCrutchCode
            var prevItem = this.listItemsStates.GetItems().Cast<ItemView>().FirstOrDefault(x => x.StrokeVisible);
            if (prevItem == null)
                prevItem = this.listItemsStates.GetSelectedItems().FirstOrDefault() as ItemView;
            if (prevItem == null)
                prevItem = this.listItemsStates.GetItems().LastOrDefault() as ItemView;
            if (prevItem != null)
            {
                prevItem.StrokeVisible = false;
                var index = this.listItemsStates.GetItems().ToList().IndexOf(prevItem) - (args.Value / Math.Abs(args.Value));
                if (index == this.listItemsStates.GetItems().Length)
                    index = 0;
                else if (index == -1)
                    index = this.listItemsStates.GetItems().Length - 1;
                var targetItem = (ItemView)this.listItemsStates.GetItems().ElementAt(index);
                targetItem.StrokeVisible = true;
            }
        }

        public StatusViewSwitch(SwitchScenarioModel scenarioModel) : this()
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
                    StateChanged?.Invoke(this, new EventsArgs<StateChangedSource>(selectedItem.StrokeVisible ? StateChangedSource.VolumeButton : StateChangedSource.Tap));
                }
            };
        }

        public void Dispose()
        {
            if (_changer != null)
            {
                _changer.VolumeDown -= _changer_VolumeChanged;
                _changer.VolumeUp -= _changer_VolumeChanged;
            }
        }

        public event EventsHandler<StateChangedSource> StateChanged;

        public enum StateChangedSource
        {
            Tap,
            VolumeButton
        }
    }

}
