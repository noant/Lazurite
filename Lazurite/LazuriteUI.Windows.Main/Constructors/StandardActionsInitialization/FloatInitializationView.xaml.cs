using Lazurite.ActionsDomain;
using Lazurite.ActionsDomain.ValueTypes;
using LazuriteUI.Windows.Controls;
using System;
using System.Windows.Controls;

namespace LazuriteUI.Windows.Main.Constructors.StandardActionsInitialization
{
    /// <summary>
    /// Логика взаимодействия для FloatInitializationView.xaml
    /// </summary>
    public partial class FloatInitializationView : UserControl, IStandardVTActionEditView
    {
        public FloatInitializationView(IStandardValueAction action, IAction masterAction=null)
        {
            InitializeComponent();

            tbMax.Validation = (v) => EntryViewValidation.DoubleValidation(min: double.Parse(tbMin.Text)).Invoke(v);
            tbMin.Validation = (v) => EntryViewValidation.DoubleValidation(max: double.Parse(tbMax.Text)).Invoke(v);
            tbVal.Validation = (v) => EntryViewValidation.DoubleValidation(max: double.Parse(tbMax.Text), min: double.Parse(tbMin.Text)).Invoke(v);

            tbMax.TextChanged += (o, e) =>
            {
                if (double.Parse(tbMax.Text) < double.Parse(tbVal.Text))
                    tbVal.Text = tbMax.Text;
            };
            
            tbMin.TextChanged += (o, e) =>
            {
                if (double.Parse(tbMin.Text) > double.Parse(tbVal.Text))
                    tbVal.Text = tbMin.Text;
            };

            btApply.Click += (o, e) => {
                action.ValueType.AcceptedValues[1] = tbMax.Text;
                action.ValueType.AcceptedValues[0] = tbMin.Text;
                ((FloatValueType)action.ValueType).Unit = tbUnit.Text;
                action.Value = tbVal.Text;
                ApplyClicked?.Invoke();
            };

            if (masterAction != null && masterAction.ValueType is FloatValueType)
            {
                tbMin.Text = masterAction.ValueType.AcceptedValues[0];
                tbMax.Text = masterAction.ValueType.AcceptedValues[1];
                tbMax.IsEnabled = tbMin.IsEnabled = false;
            }
            else
            {
                tbMin.Text = action.ValueType.AcceptedValues[0];
                tbMax.Text = action.ValueType.AcceptedValues[1];
            }

            tbVal.Text = action.Value;
            tbUnit.Text = ((FloatValueType)action.ValueType).Unit;
        }

        public event Action ApplyClicked;
    }
}
