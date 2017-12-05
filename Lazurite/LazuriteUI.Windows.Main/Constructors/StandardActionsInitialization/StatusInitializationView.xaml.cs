using Lazurite.ActionsDomain;
using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.CoreActions.StandardValueTypeActions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LazuriteUI.Windows.Main.Constructors.StandardActionsInitialization
{
    /// <summary>
    /// Логика взаимодействия для StatusInitializationView.xaml
    /// </summary>
    public partial class StatusInitializationView : UserControl, IStandardVTActionEditView
    {
        private IStandardValueAction _action;
        private IAction _masterAction;
        private StateValueType _valueType;
        private string _oldVal;
        public StatusInitializationView(IStandardValueAction action, IAction masterAction = null)
        {
            InitializeComponent();
            _masterAction = masterAction;
            _action = action;
            _oldVal = _action.Value;
            _valueType = (StateValueType)action.ValueType;
            
            if (masterAction != null)
            {
                gridAdd.Visibility = Visibility.Collapsed;
                _valueType = (StateValueType)masterAction.ValueType;
            }
            else
            {
                btAddNew.Click += (o, e) =>
                {
                    if (!string.IsNullOrWhiteSpace(tbNewStatus.Text) &&
                        !listItemsStatus.Children.Cast<StatusInitializationViewItem>().Any(x => x.Text.Equals(tbNewStatus.Text)))
                    {
                        Add(tbNewStatus.Text);
                        tbNewStatus.Text = string.Empty;
                    }
                };
            }

            foreach (var state in _valueType.AcceptedValues)
                Add(state);

            btApply.Click += (o, e) => {
                var selectedState = (listItemsStatus.SelectedItem as StatusInitializationViewItem)?.Text;
                if (string.IsNullOrEmpty(selectedState))
                    selectedState = _oldVal;
                var states = listItemsStatus.GetItems().Select(x => ((StatusInitializationViewItem)x).Text);
                _valueType.AcceptedValues = states.ToArray();
                _action.ValueType = _valueType;
                _action.Value = selectedState;
                ApplyClicked?.Invoke();
            };
        }

        private void Add(string state)
        {
            var itemView = new StatusInitializationViewItem();
            itemView.Margin = new Thickness(0, 0, 0, 1);
            itemView.Text = state;
            itemView.IsRemoveButtonVisible = _masterAction == null;
            if (_action.Value == state)
                this.Loaded += (o, e) => itemView.Selected = true;
            itemView.RemoveClick += (o, args) => listItemsStatus.Children.Remove(itemView);
            listItemsStatus.Children.Add(itemView);
        }

        public event Action ApplyClicked;
    }
}
