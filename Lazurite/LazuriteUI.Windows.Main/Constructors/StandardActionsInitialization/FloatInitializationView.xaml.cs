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
    /// Логика взаимодействия для FloatInitializationView.xaml
    /// </summary>
    public partial class FloatInitializationView : UserControl, IStandardVTActionEditView
    {
        public FloatInitializationView(IStandardValueAction action, IAction masterAction=null)
        {
            InitializeComponent();

            tbMax.Validation = (str) => double.Parse(str) > double.Parse(tbMin.Text);
            tbMin.Validation = (str) => double.Parse(str) < double.Parse(tbMax.Text);
            tbVal.Validation = (str) => double.Parse(tbMin.Text) <= double.Parse(str) && double.Parse(str) <= double.Parse(tbMax.Text);

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
                action.Value = tbVal.Text;
                ApplyClicked?.Invoke();
            };

            if (masterAction != null)
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
        }

        public event Action ApplyClicked;
    }
}
