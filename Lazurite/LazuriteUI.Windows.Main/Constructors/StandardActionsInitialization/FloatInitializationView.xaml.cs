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
        public FloatInitializationView(IAction action = null, IAction masterAction=null)
        {
            InitializeComponent();

            tbMax.Validation = (str) => double.Parse(str) > double.Parse(tbMin.Text);
            tbMin.Validation = (str) => double.Parse(str) < double.Parse(tbMax.Text);
            tbVal.Validation = (str) => double.Parse(tbMin.Text) <= double.Parse(str) && double.Parse(str) <= double.Parse(tbMax.Text);
            
            btApply.Click += (o, e) => {
                ((FloatValueType)action.ValueType).AcceptedValues[1] = tbMax.Text;
                ((FloatValueType)action.ValueType).AcceptedValues[0] = tbMin.Text;
                ((GetFloatVTAction)action).Value = tbVal.Text;
                ApplyClicked?.Invoke();
            };
        }

        public event Action ApplyClicked;
    }
}
