using Lazurite.ActionsDomain;
using Lazurite.ActionsDomain.ValueTypes;
using System;
using System.Windows.Controls;

namespace LazuriteUI.Windows.Main.Constructors.StandardActionsInitialization
{
    /// <summary>
    /// Логика взаимодействия для ToggleInitializationView.xaml
    /// </summary>
    public partial class ToggleInitializationView : UserControl, IStandardVTActionEditView
    {
        public ToggleInitializationView(IStandardValueAction action, IAction masterAction = null)
        {
            InitializeComponent();
            if (action.Value == ToggleValueType.ValueON)
                btOn.Selected = true;
            else btOff.Selected = true;

            btOn.Click += (o, e) => {
                action.Value = ToggleValueType.ValueON;
                ApplyClicked?.Invoke();
            };
            btOff.Click += (o, e) => {
                action.Value = ToggleValueType.ValueOFF;
                ApplyClicked?.Invoke();
            };
        }

        public event Action ApplyClicked;
    }
}
