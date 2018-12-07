using Lazurite.IOC;
using Lazurite.MainDomain;
using Lazurite.Shared;
using Lazurite.Utils;
using LazuriteMobile.App.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace LazuriteMobile.App.Switches
{
    public partial class StatusViewSwitch : Grid, IDisposable
    {
        private static readonly int FloatView_ValueUpdateInterval = GlobalSettings.Get(300);
        private static ISystemUtils SystemUtils = Singleton.Resolve<ISystemUtils>();
        
        private IHardwareVolumeChanger _changer;
        private SwitchScenarioModel _model;
        private string _currentVal;
        private ItemView _prevItem;
        private Dictionary<string, ItemView> _visibleItems = new Dictionary<string, ItemView>();

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
            if (_model.AcceptedValues.Length > 1)
            {
                var direction = args.Value > 0 ? -1 : 1;

                var currentValues = listView.ItemsSource as string[];

                var currentIndex = currentValues.ToList().IndexOf(_currentVal);
                var nextIndex = currentIndex + direction;
                if (nextIndex >= currentValues.Length)
                    nextIndex = 0;
                else if (nextIndex < 0)
                    nextIndex = currentValues.Length - 1;

                _currentVal = currentValues[nextIndex];

                if (_prevItem != null)
                    _prevItem.StrokeVisible = false;
                listView.ScrollTo(_currentVal, ScrollToPosition.MakeVisible, false);
                if (_visibleItems.ContainsKey(_currentVal))
                {
                    var itemView = _visibleItems[_currentVal];
                    _prevItem = itemView;
                    itemView.StrokeVisible = true;
                }
            }
        }

        public StatusViewSwitch(SwitchScenarioModel scenarioModel) : this()
        {
            BindingContext = _model = scenarioModel;

            _currentVal = _model.ScenarioValue;

            listView.ItemsSource = GetItemsSource();
            
            if (_model.AcceptedValues.Length > 10)
                tbSearch.Completed += (o, e) =>
                    listView.ItemsSource = GetItemsSource();
            else
            {
                tbSearch.IsVisible = false;
                iconSearch.IsVisible = false;
            }

            // Невозможно замапить объекты, так как, почему-то, сбивается размер всего контрола.
            // Если мапить строки, то размер нормальный.

            listView.ItemTemplate = new DataTemplate(() => {
                var itemView = new ItemView();
                itemView.SetBinding(ItemView.TextProperty, ".");
                itemView.Icon = LazuriteUI.Icons.Icon.ChevronRight;
                itemView.Selectable = true;
                itemView.StrokeVisibilityClick = true;

                itemView.SelectionChanged += (o, e) => {
                    if (itemView.Selected)
                    {
                        foreach (var item in _visibleItems.Values)
                            if (item != itemView && item.Selected)
                                item.Selected = false;
                    }
                };

                itemView.PropertyChanged += (o, e) => {
                    if (e.PropertyName == nameof(itemView.Text))
                    {
                        if (itemView.Text == _model.ScenarioValue)
                            itemView.Selected = true;
                        else if (itemView.Selected)
                            itemView.Selected = false;
                    }
                };

                itemView.Click += (o, e) => RaiseSelect(itemView.Text, (ItemView.ClickSource)e.Value);

                var viewCell = new ViewCell();

                viewCell.Appearing += (o, e) =>
                {
                    if (itemView.Text != null)
                    {
                        if (_visibleItems.ContainsKey(itemView.Text))
                            _visibleItems.Remove(itemView.Text);
                        _visibleItems.Add(itemView.Text, itemView);
                    }
                };
                viewCell.Disappearing += (o, e) =>
                {
                    if (itemView.Text != null && _visibleItems.ContainsKey(itemView.Text))
                        _visibleItems.Remove(itemView.Text);
                };
                
                viewCell.View = itemView;
                return viewCell;
            });
        }

        private string[] GetItemsSource()
        {
            var searchText = tbSearch.Text?.Trim().ToLowerInvariant();

            if (string.IsNullOrEmpty(searchText))
                return _model.AcceptedValues;

            return _model
                .AcceptedValues
                .Where(x => x.ToLowerInvariant().Contains(searchText))
                .ToArray();
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            if (_model != null)
                listView.ScrollTo(_model.ScenarioValue, ScrollToPosition.MakeVisible, false);
            base.OnSizeAllocated(width, height);
        }

        private void RaiseSelect(string value, ItemView.ClickSource source)
        {
            _currentVal = _model.ScenarioValue = value;
            StateChanged?.Invoke(this, new EventsArgs<ItemView.ClickSource>(source));
        }

        public void Dispose()
        {
            if (_changer != null)
            {
                _changer.VolumeDown -= _changer_VolumeChanged;
                _changer.VolumeUp -= _changer_VolumeChanged;
            }
        }

        public event EventsHandler<ItemView.ClickSource> StateChanged;
    }

}
