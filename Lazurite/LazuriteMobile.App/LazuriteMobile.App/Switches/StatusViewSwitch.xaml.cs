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
        private static Dictionary<string, string> SearchCache = new Dictionary<string, string>();

        private IHardwareVolumeChanger _changer;
        private SwitchScenarioModel _model;
        private string _currentVal;
        private ItemViewFast _prevItem;
        private Dictionary<string, ItemViewFast> _visibleItems = new Dictionary<string, ItemViewFast>();

        public StatusViewSwitch()
        {
            InitializeComponent();

            // Extra f****** crutch, list view calculates wrong height
            bool crutch = true;
            listView.SizeChanged += (o, e) => {
                if (crutch)
                {
                    crutch = false;
                    listView.HeightRequest = listView.Height + 5;
                }
            };

            if (Singleton.Any<IHardwareVolumeChanger>())
            {
                _changer = Singleton.Resolve<IHardwareVolumeChanger>();
                _changer.VolumeDown += _changer_VolumeChanged;
                _changer.VolumeUp += _changer_VolumeChanged;
            }
        }

        private static string GetSearchCache(string scenarioId)
        {
            if (SearchCache.ContainsKey(scenarioId))
                return SearchCache[scenarioId] ?? string.Empty;
            else
                return string.Empty;
        }

        private static void SetSearchCache(string scenarioId, string searchString)
        {
            if (SearchCache.ContainsKey(scenarioId))
                SearchCache[scenarioId] = searchString;
            else
                SearchCache.Add(scenarioId, searchString);
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

                ScrollTo(_currentVal);
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
            //megaCrutchCode

            BindingContext = _model = scenarioModel;

            _model.PropertyChanged += _model_PropertyChanged;

            _currentVal = _model.ScenarioValue;

            listView.ItemsSource = GetItemsSource();

            if (_model.AcceptedValues.Length > 10)
            {
                tbSearch.Text = GetSearchCache(_model.Scenario.ScenarioId);
                HandleSearch();

                tbSearch.Completed += (o, e) => HandleSearch();

                btClearSearch.Click += (o, e) =>
                {
                    tbSearch.Text = string.Empty;
                    SetSearchCache(_model.Scenario.ScenarioId, string.Empty);
                    btClearSearch.IsVisible = false;
                    listView.ItemsSource = GetItemsSource();
                };

                iconTitle.IsVisible = false;
                lblTitle.IsVisible = false;
            }
            else
            {
                iconSearch.IsVisible = false;
                tbSearch.IsVisible = false;
            }

            // Невозможно замапить объекты, так как, почему-то, сбивается размер всего контрола.
            // Если мапить строки, то размер нормальный.

            listView.ItemTemplate = new DataTemplate(() => {
                var itemView = new ItemViewFast();
                itemView.SetBinding(ItemViewFast.TextProperty, ".");
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

                        itemView.Selected = itemView.Text == _model.ScenarioValue;
                    }
                };
                viewCell.Disappearing += (o, e) =>
                {
                    if (itemView.Text != null && _visibleItems.ContainsKey(itemView.Text))
                        _visibleItems.Remove(itemView.Text);

                    itemView.Selected = false;
                };
                viewCell.View = itemView;
                return viewCell;
            });
        }

        private void _model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_model.ScenarioValue) && _currentVal != _model.ScenarioValue)
            {
                _currentVal = _model.ScenarioValue;
                ScrollTo(_currentVal);
                if (_visibleItems.ContainsKey(_currentVal))
                    _visibleItems[_currentVal].Selected = true;
            }
        }

        private void ScrollTo(string val)
        {
            listView.ScrollTo(val, ScrollToPosition.Center, false);
        }

        private void HandleSearch()
        {
            SetSearchCache(_model.Scenario.ScenarioId, tbSearch.Text);
            btClearSearch.IsVisible = !string.IsNullOrEmpty(tbSearch.Text);
            listView.ItemsSource = GetItemsSource();
        }

        private string[] GetItemsSource()
        {
            var searchText = GetSearchCache(_model.Scenario.ScenarioId).Trim().ToLowerInvariant();

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
                ScrollTo(_model.ScenarioValue);
            base.OnSizeAllocated(width, height);
        }

        private void RaiseSelect(string value, ItemView.ClickSource source)
        {
            _model.ScenarioValue  = _currentVal = value;
            StateChanged?.Invoke(this, new EventsArgs<ItemView.ClickSource>(source));
        }

        public void Dispose()
        {
            if (_changer != null)
            {
                _changer.VolumeDown -= _changer_VolumeChanged;
                _changer.VolumeUp -= _changer_VolumeChanged;
            }
            _model.PropertyChanged -= _model_PropertyChanged;
        }

        public event EventsHandler<ItemView.ClickSource> StateChanged;
    }

}
