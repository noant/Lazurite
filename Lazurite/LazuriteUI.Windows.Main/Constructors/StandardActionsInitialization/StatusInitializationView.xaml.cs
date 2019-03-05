using Lazurite.ActionsDomain;
using Lazurite.ActionsDomain.ValueTypes;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace LazuriteUI.Windows.Main.Constructors.StandardActionsInitialization
{
    /// <summary>
    /// Логика взаимодействия для StatusInitializationView.xaml
    /// </summary>
    public partial class StatusInitializationView : UserControl, IStandardVTActionEditView
    {
        private IStandardValueAction _action;
        private IAction _masterAction;
        private ValueTypeBase _valueType;
        private string _oldVal;

        public StatusInitializationView(IStandardValueAction action, IAction masterAction = null)
        {
            InitializeComponent();
            _masterAction = masterAction;
            _action = action;
            _oldVal = _action.Value;
            _valueType = action.ValueType;

            if (masterAction != null)
            {
                gridAdd.Visibility = Visibility.Collapsed;
                _valueType = masterAction.ValueType;
            }
            else
            {
                btAddNew.Click += (o, e) => CreateNewStateFromTextBox();
                tbNewStatus.KeyDown += (o, e) =>
                {
                    if (e.Key == System.Windows.Input.Key.Enter)
                    {
                        CreateNewStateFromTextBox();
                    }
                };
            }

            foreach (var state in _valueType.AcceptedValues)
            {
                Add(state);
            }

            UpdateSearchControls();

            btApply.Click += (o, e) =>
            {
                var selectedState = (listItemsStatus.SelectedItem as StatusInitializationViewItem)?.Text;
                if (string.IsNullOrEmpty(selectedState))
                {
                    selectedState = _oldVal;
                }

                var states = listItemsStatus.GetItems().Select(x => ((StatusInitializationViewItem)x).Text);
                _valueType.AcceptedValues = states.ToArray();
                _action.ValueType = _valueType;
                _action.Value = selectedState;
                ApplyClicked?.Invoke();
            };
        }

        private void CreateNewStateFromTextBox()
        {
            if (!string.IsNullOrWhiteSpace(tbNewStatus.Text) &&
                !listItemsStatus.Children.Cast<StatusInitializationViewItem>().Any(x => x.Text.Equals(tbNewStatus.Text)))
            {
                Add(tbNewStatus.Text);
                tbNewStatus.Text = string.Empty;
                UpdateSearchControls();
            }
        }

        private void UpdateSearchControls()
        {
            if (listItemsStatus.Children.Count < 7)
            {
                gridSearchControls.Visibility = Visibility.Collapsed;
            }
            else
            {
                gridSearchControls.Visibility = Visibility.Visible;
            }
        }

        private void Add(string state)
        {
            var itemView = new StatusInitializationViewItem();
            itemView.Margin = new Thickness(0, 0, 0, 1);
            itemView.Text = state;
            itemView.IsRemoveButtonVisible = _masterAction == null;
            if (_action.Value == state)
            {
                Loaded += (o, e) => itemView.Selected = true;
            }

            itemView.RemoveClick += (o, args) =>
            {
                listItemsStatus.Children.Remove(itemView);
                UpdateSearchControls();
            };
            listItemsStatus.Children.Add(itemView);
        }

        public event Action ApplyClicked;

        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textToSearch = tbSearch.Text.ToLower();
            var empty = string.IsNullOrWhiteSpace(textToSearch);
            foreach (StatusInitializationViewItem item in listItemsStatus.Children)
            {
                if (empty || item.Text.ToLower().Contains(textToSearch))
                {
                    item.Visibility = Visibility.Visible;
                }
                else
                {
                    item.Visibility = Visibility.Collapsed;
                }
            }
        }
    }
}