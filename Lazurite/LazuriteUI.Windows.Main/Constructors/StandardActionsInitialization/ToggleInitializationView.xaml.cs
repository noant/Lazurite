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
